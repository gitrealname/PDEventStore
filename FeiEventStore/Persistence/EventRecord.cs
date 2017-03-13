﻿namespace FeiEventStore.Persistence
{
    using System;
    using FeiEventStore.Core;

    /// <summary>
    /// member of this class MUST not be serialized, it is used for EventStore to PersistenceEngine communication only!!!
    /// </summary>
    public class EventRecordHeader
    {
        /// <summary>
        /// Gets or sets a value indicating whether to process primary key.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [process primary key]; otherwise, <c>false</c>.
        /// </value>
        public bool ProcessPrimaryKey { get; set; }

    }
    public class EventRecord
    {

        /// <summary>
        /// Gets or sets the header.
        /// IMPORTANT: is not to be persisted!!!
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public EventRecordHeader Header { get; set; } = new EventRecordHeader();

        /// <summary>
        /// Set by the engine
        /// </summary>
        /// <value>
        /// The sequence.
        /// </value>
        /// <summary>
        /// Returns most recent store version.
        /// </summary>
        public long StoreVersion { get; set; }

        public Guid? OriginUserId { get; set; }

        public Guid? OriginSystemId { get; set; }

        public Guid AggregateId { get; set; }

        public long AggregateVersion { get; set; }

        public TypeId AggregateTypeId { get; set; }

        /// <summary>
        /// Gets or sets the Unique Key per Aggregate Type. 
        /// </summary>
        /// <value>
        /// The aggregate key.
        /// </value>
        public string AggregateTypeUniqueKey { get; set; }


        /// <summary>
        /// Gets or sets the event type identifier.
        /// In case of upgraded event, this must be an original (oldest aggregate type id)
        /// </summary>
        /// <value>
        /// The event type identifier.
        /// </value>
        public TypeId EventPayloadTypeId { get; set; }

        /// <summary>
        /// Gets or sets the event Creation Timestamp.
        /// </summary>
        /// <value>
        /// The event timestamp.
        /// </value>
        public DateTimeOffset EventTimestamp { get; set; }

        public object Payload { get; set; }
    }
}