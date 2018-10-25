$secretsPath = ".\UwpSample\~Secrets.cs";
((Get-Content -path $secretsPath -Raw) -replace '<HUB_NAME>',$env:HUB_NAME) | Set-Content -Path $secretsPath
((Get-Content -path $secretsPath -Raw) -replace '<HUB_CONNECTION_STRING>',$env:HUB_CONNECTION_STRING) | Set-Content -Path $secretsPath
