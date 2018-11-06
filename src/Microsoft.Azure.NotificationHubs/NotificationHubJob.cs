//------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for 
// license information.
//------------------------------------------------------------

namespace Microsoft.Azure.NotificationHubs
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.Azure.NotificationHubs.Messaging;
    
    /// <summary>
    /// Allowed export/import job types when bulk registration operations are to be done
    /// </summary>
    public enum NotificationHubJobType
    {
        /// <summary>
        /// Job type to bulk get registrations 
        /// </summary>
        ExportRegistrations = 0,

        /// <summary>
        /// Job type to bulk create registrations  
        /// </summary>
        ImportCreateRegistrations = 1,

        /// <summary>
        /// Job type to bulk update registrations  
        /// </summary>
        ImportUpdateRegistrations = 2,

        /// <summary>
        /// Job type to bulk delete registrations
        /// </summary>
        ImportDeleteRegistrations = 3,

        /// <summary>
        /// Job type to bulk upsert registrations
        /// </summary>
        ImportUpsertRegistrations = 4
    }


    /// <summary>
    /// Returns the status of a Notification Hub Job.
    /// </summary>
    public enum NotificationHubJobStatus
    {
        /// <summary>
        /// Indicates that the NotificationHubJob was accepted.
        /// </summary>
        Started = 0,

        /// <summary>
        /// Indicates that the NotificationHubJob is currently running. Depending on the amount of data,
        /// a job may stay in this state for several hours.
        /// </summary>
        Running = 1,

        /// <summary>
        /// Indicates that the NotificationHubJob was completed successfully. Any output
        /// will be ready where configured via the NotificationHubJob object.
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Indicates that the NotificationHubJob has failed. Information on the failure
        /// can be found in the <see cref="Microsoft.Azure.NotificationHubs.NotificationHubJob.FailuresFileName"/> file.
        /// </summary>
        Failed = 3
    }
    


    /// <summary>
    /// Metadata of the NotificationHub Job
    /// </summary>
    [DataContract(Name = ManagementStrings.NotificationHubJob, Namespace = ManagementStrings.Namespace)]
    public sealed class NotificationHubJob : EntityDescription
    {

        /// <summary>
        /// Gets the job identifier.
        /// </summary>
        /// <value>
        /// The job identifier.
        /// </value>
        [DataMember(Name = ManagementStrings.NotificationHubJobId, IsRequired = false, Order = 1001, EmitDefaultValue = false)]
        public string JobId
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        /// <value>
        /// The name of the collection.
        /// </value>
        public string CollectionName
        {
            get { return "jobs"; }
        }

        /// <summary>
        /// Gets the name of the output file.
        /// </summary>
        /// <value>
        /// The name of the output file.
        /// </value>
        public string OutputFileName
        {
            get
            {
                string path = string.Empty;
                var outputProperties = this.OutputProperties;
                if (outputProperties != null)
                {
                    outputProperties.TryGetValue(ManagementStrings.OutputFilePath, out path);
                }

                return path;
            }
        }
        
        /// <summary>
        /// Gets the name of the failures file.
        /// </summary>
        /// <value>
        /// The name of the failures file.
        /// </value>
        public string FailuresFileName
        {
            get
            {
                string path = string.Empty;
                var outputProperties = this.OutputProperties;
                if (outputProperties != null)
                {
                    outputProperties.TryGetValue(ManagementStrings.FailedFilePath, out path);
                }

                return path;
            }
        }

        /// <summary>
        /// Gets the progress.
        /// </summary>
        /// <value>
        /// The progress.
        /// </value>
        [DataMember(Name = ManagementStrings.Progress, IsRequired = false, Order = 1002, EmitDefaultValue = false)]
        public decimal Progress { get; internal set; }

        /// <summary>
        /// Gets or sets the type of the job.
        /// </summary>
        /// <value>
        /// The type of the job.
        /// </value>
        [DataMember(Name = ManagementStrings.JobType, IsRequired = true, Order = 1003, EmitDefaultValue = true)]
        public NotificationHubJobType JobType { get; set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [DataMember(Name = ManagementStrings.Status, IsRequired = false, Order = 1004, EmitDefaultValue = false)]
        public NotificationHubJobStatus Status { get; internal set; }

        /// <summary>
        /// Gets or sets the output container URI.
        /// </summary>
        /// <value>
        /// The output container URI.
        /// </value>
        [DataMember(Name = ManagementStrings.OutputContainerUri, IsRequired = true, Order = 1005, EmitDefaultValue = false)]
        public Uri OutputContainerUri { get; set; }

        /// <summary>
        /// Gets or sets the import file URI.
        /// </summary>
        /// <value>
        /// The import file URI.
        /// </value>
        [DataMember(Name = ManagementStrings.ImportFileUri, IsRequired = false, Order = 1006, EmitDefaultValue = false)]
        public Uri ImportFileUri { get; set; }

        /// <summary>
        /// Gets or sets the input properties.
        /// </summary>
        /// <value>
        /// The input properties.
        /// </value>
        [DataMember(Name = ManagementStrings.InputProperties, IsRequired = false, Order = 1007, EmitDefaultValue = false)]
        public Dictionary<string, string> InputProperties { get; set; }

        /// <summary>
        /// Gets the failure.
        /// </summary>
        /// <value>
        /// The failure.
        /// </value>
        [DataMember(Name = ManagementStrings.Failure, IsRequired = false, Order = 1008, EmitDefaultValue = false)]
        public string Failure { get; internal set; }

        /// <summary>
        /// Gets the output properties.
        /// </summary>
        /// <value>
        /// The output properties.
        /// </value>
        [DataMember(Name = ManagementStrings.OutputProperties, IsRequired = false, Order = 1009, EmitDefaultValue = false)]
        public Dictionary<string, string> OutputProperties { get; internal set; }

        /// <summary>
        /// Gets the created time.
        /// </summary>
        /// <value>
        /// The created time.
        /// </value>
        [DataMember(Name = ManagementStrings.CreatedAt, IsRequired = false, Order = 1010, EmitDefaultValue = false)]
        public DateTime CreatedAt { get; internal set; }

        /// <summary>
        /// Gets the updated time.
        /// </summary>
        /// <value>
        /// The updated time.
        /// </value>
        [DataMember(Name = ManagementStrings.UpdatedAt, IsRequired = false, Order = 1011, EmitDefaultValue = false)]
        public DateTime UpdatedAt { get; internal set; }
    }
}