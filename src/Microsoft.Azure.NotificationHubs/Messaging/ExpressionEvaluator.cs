//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//----------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.Azure.NotificationHubs;
    using Microsoft.Azure.NotificationHubs.Common;

    static class ExpressionEvaluator
    {
        public const string InternalBodyProperty = "READ_ONCE_BODY_PUSHNOTIFICATION";
        public const string InternalJsonNavigationProperty = "FIRST_LEVEL_JSON_NAVIGATION_PUSHNOTIFICATION";
        const string BodyExpression = @"$body";
        public const int MaxLengthOfPropertyName = 120;
        public static readonly Regex PropertyNameRegEx = new Regex("^[A-Za-z0-9_]+$");

        enum TokenType
        {
            None,
            Dollar,
            Hash,
            Dot,
            Percentage,
            SingleLiteral,
            DoubleLiteral,
            Body
        }

        public enum ExpressionType
        {
            Literal,
            Numeric,
            String,
            Composite,
            None
        }

        public static ExpressionType Validate(string expression)
        {
            ExpressionType expressionType;
            List<Token> tokens = ValidateAndTokenize(expression, out expressionType);

            Token bodyToken = tokens.Find((t) => { return t.Type == TokenType.Body; });
            if (bodyToken != null)
            {
                throw new InvalidDataContractException(SRClient.BodyIsNotSupportedExpression);
            }

            return expressionType;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        static List<Token> ValidateAndTokenize(string expression, out ExpressionType expressionType)
        {
            expressionType = ExpressionEvaluator.PeekExpressionType(expression);

            if (expressionType == ExpressionType.Literal)
            {
                return new List<Token>();
            }

            List<Token> tokens = new List<Token>();
            string workingInput = expression;
            int tokenEndIndex;

            if (expressionType == ExpressionType.Composite)
            {
                if (expression[expression.Length - 1] != '}')
                {
                    throw new InvalidDataContractException(string.Format(SRClient.ExpressionMissingClosingParenthesesNoToken, expression));
                }

                workingInput = expression.Substring(1, expression.Length - 2).TrimEnd();
            }

            while (true)
            {
                Token token = new Token();
                workingInput = workingInput.TrimStart();

                if (workingInput.Length < 3)
                {
                    throw new InvalidDataContractException(string.Format(SRClient.ExpressionInvalidToken, expression, workingInput));
                }

                switch (workingInput[0])
                {
                    case '.':
                        token.Type = TokenType.Dot;
                        tokenEndIndex = ExpressionEvaluator.ExtractToken(expression, workingInput, token);
                        break;
                    case '%':
                        token.Type = TokenType.Percentage;
                        tokenEndIndex = ExpressionEvaluator.ExtractToken(expression, workingInput, token);
                        break;
                    case '\'':
                        token.Type = TokenType.SingleLiteral;
                        tokenEndIndex = ExpressionEvaluator.ExtractLiteral(expression, workingInput, token);
                        break;
                    case '"':
                        token.Type = TokenType.DoubleLiteral;
                        tokenEndIndex = ExpressionEvaluator.ExtractLiteral(expression, workingInput, token);
                        break;
                    case '#':
                        token.Type = TokenType.Hash;
                        tokenEndIndex = ExpressionEvaluator.ExtractToken(expression, workingInput, token);
                        break;
                    case '$':
                        if (workingInput.ToLowerInvariant().StartsWith(BodyExpression, StringComparison.OrdinalIgnoreCase))
                        {
                            tokenEndIndex = BodyExpression.Length - 1;
                            token.Type = TokenType.Body;
                        }
                        else
                        {
                            token.Type = TokenType.Dollar;
                            tokenEndIndex = ExpressionEvaluator.ExtractToken(expression, workingInput, token);
                        }

                        break;
                    default:
                        throw new InvalidDataContractException(string.Format(SRClient.ExpressionInvalidTokenType, expression, workingInput));
                }

                if (token.Type != TokenType.SingleLiteral &&
                    token.Type != TokenType.DoubleLiteral &&
                    token.Type != TokenType.Body &&
                    !string.Equals(token.Property, ExpressionEvaluator.BodyExpression, StringComparison.OrdinalIgnoreCase))
                {
                    if (!ExpressionEvaluator.PropertyNameRegEx.IsMatch(token.Property))
                    {
                        throw new InvalidDataContractException(string.Format(SRClient.PropertyNameIsBad, token.Property));
                    }

                    if (token.Property.Length > ExpressionEvaluator.MaxLengthOfPropertyName)
                    {
                        throw new InvalidDataContractException(string.Format(SRClient.PropertyTooLong, token.Property.Length, ExpressionEvaluator.MaxLengthOfPropertyName));
                    }
                }

                tokens.Add(token);

                if (workingInput.Length == tokenEndIndex + 1)
                {
                    break;
                }

                workingInput = workingInput.Substring(tokenEndIndex + 1).TrimStart();

                if (workingInput[0] != '+')
                {
                    throw new InvalidDataContractException(string.Format(SRClient.ExpressionInvalidCompositionOperator, expression, workingInput));
                }

                workingInput = workingInput.Substring(1);
            }

            if (tokens.Count > 1 && tokens.Find((token) => token.Type == TokenType.Hash) != null)
            {
                throw new InvalidDataContractException(SRClient.ExpressionHashInComposite);
            }

            return tokens;
        }

        static string Evaluate(List<string> values)
        {
            if (values.Count == 1)
            {
                return values[0];
            }

            int expectedStringSize = 0;
            values.ForEach(s => expectedStringSize += s.Length);

            StringBuilder finalString = new StringBuilder(expectedStringSize);
            values.ForEach(s => finalString.Append(s));

            return finalString.ToString();
        }

        static ExpressionType PeekExpressionType(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return ExpressionType.Literal;
            }

            char firstChar = expression[0];
            switch (firstChar)
            {
                case '$':
                case '.':
                case '%':
                    return ExpressionType.String;

                case '#':
                    return ExpressionType.Numeric;

                case '{':
                    return ExpressionType.Composite;

                default:
                    return ExpressionType.Literal;
            }
        }

        static int ExtractLiteral(string fullExpression, string tokenBegin, Token token)
        {
            int tokenEndIndex = 0;
            int startLookingFromIndex = 1;
            char literalSeperator = token.Type == TokenType.SingleLiteral ? '\'' : '\"';
            bool escapeCharsFound = false;

            while (true)
            {
                tokenEndIndex = tokenBegin.IndexOf(literalSeperator, startLookingFromIndex);

                if (tokenEndIndex == -1)
                {
                    throw new InvalidDataContractException(string.Format(SRClient.ExpressionLiteralMissingClosingNotation, fullExpression, tokenBegin));
                }

                if (tokenEndIndex + 1 < tokenBegin.Length && tokenBegin[tokenEndIndex + 1] == literalSeperator)
                {
                    escapeCharsFound = true;
                    startLookingFromIndex = tokenEndIndex + 2;
                    continue;
                }
                else
                {
                    string literal = tokenBegin.Substring(1, tokenEndIndex - 1);

                    if (escapeCharsFound)
                    {
                        token.Property = token.Type == TokenType.DoubleLiteral ? literal.Replace(@"""""", @"""") : literal.Replace(@"''", @"'");
                    }
                    else
                    {
                        token.Property = literal;
                    }

                    break;
                }
            }

            return tokenEndIndex;
        }

        static int ExtractToken(string fullExpression, string tokenBegin, Token token)
        {
            // Cant find opening Parentheses  
            if (tokenBegin[1] != '(')
            {
                throw new InvalidDataContractException(string.Format(SRClient.ExpressionMissingOpenParentheses, fullExpression, tokenBegin));
            }

            int indexOfClose = tokenBegin.IndexOf(')');
            int indexOfComma = tokenBegin.IndexOf(',');

            int indexOfDefaultBegin = tokenBegin.IndexOf(":{", StringComparison.InvariantCultureIgnoreCase);
            int indexOfDefaultEnd = indexOfDefaultBegin + 2;
            int defaultFullLength = 0;

            // Here we 'read' default value which may contain any character
            if (indexOfDefaultBegin > 1 && indexOfDefaultBegin < indexOfClose)
            {
                if (indexOfDefaultBegin < 3)
                {
                    throw new InvalidDataContractException(string.Format(SRClient.ExpressionMissingProperty, fullExpression, tokenBegin));
                }

                var escape = false;
                while (true)
                {
                    if (indexOfDefaultEnd >= tokenBegin.Length)
                    {
                        throw new InvalidDataContractException(string.Format(SRClient.ExpressionMissingDefaultEnd, fullExpression, tokenBegin));
                    }

                    if (tokenBegin[indexOfDefaultEnd] == '}' && !escape)
                    {
                        break;
                    }

                    escape = tokenBegin[indexOfDefaultEnd] == '\\' && !escape;
                    indexOfDefaultEnd++;
                }

                defaultFullLength = indexOfDefaultEnd - indexOfDefaultBegin + 1;
                token.DefaultValue = tokenBegin.Substring(indexOfDefaultBegin + 2, defaultFullLength - 3);

                // Fix indexes if default value contains ')' or ','
                indexOfComma = tokenBegin.IndexOf(',', indexOfDefaultEnd);
                indexOfClose = tokenBegin.IndexOf(')', indexOfDefaultEnd);
            }

            // Cant find closing Parentheses  
            if (indexOfClose == -1)
            {
                throw new InvalidDataContractException(string.Format(SRClient.ExpressionMissingClosingParentheses, fullExpression, tokenBegin));
            }

            // When percentage or hash is used comma is prohibited
            if ((token.Type == TokenType.Percentage || token.Type == TokenType.Hash) && indexOfComma != -1 && indexOfComma < indexOfClose)
            {
                throw new InvalidDataContractException(string.Format(SRClient.ExpressionErrorParsePercentFormat, fullExpression, tokenBegin));
            }

            // When dot is used comma is mandatory
            if (token.Type == TokenType.Dot && indexOfComma == -1)
            {
                throw new InvalidDataContractException(string.Format(SRClient.ExpressionErrorParseDotFormat, fullExpression, tokenBegin));
            }

            int shortLength = 0;
            if (indexOfComma != -1 && indexOfComma < indexOfClose)
            {
                string shortLengthString = tokenBegin.Substring(indexOfComma + 1, indexOfClose - indexOfComma - 1);

                if (!int.TryParse(shortLengthString, out shortLength) || shortLength < 0)
                {
                    throw new InvalidDataContractException(string.Format(SRClient.ExpressionIsNotPositiveInteger, fullExpression, tokenBegin, shortLengthString));
                }

                if (shortLength == 0)
                {
                    token.EmptyString = true;
                }

                token.Length = shortLength;
                token.Property = tokenBegin.Substring(2, indexOfComma - 2 - defaultFullLength);
            }
            else
            {
                token.Property = tokenBegin.Substring(2, indexOfClose - 2 - defaultFullLength);
            }

            token.Property = token.Property.Trim();
            return indexOfClose;
        }

        class Token
        {
            public bool EmptyString { get; set; }

            public int Length { get; set; }

            public string Property { get; set; }
            
            public string DefaultValue { get; set; }

            public TokenType Type { get; set; }
        }
    }
}
