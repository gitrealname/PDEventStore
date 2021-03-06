﻿using FeiEventStore.Core;

namespace FeiEventStore.Persistence
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// SQL based engine implementation requirements:
    ///     1. <see cref="EventRecord.StoreVersion"/>Store Version should be a primary key.
    ///         Violation of the this key should result in <see cref="EventStoreConcurrencyViolationException"/>,
    ///         which can be used by event store to re-try the commit.
    ///         
    ///     2. It has to be an unique index on <see cref="EventRecord.AggregateId"/> and <see cref="EventRecord.AggregateVersion"/>,
    ///         This is required to perform lookup and load an Aggregate. 
    ///         Violation of this index is not expected as all collisions are handled using #1.
    ///         
    ///     3. It has to be an unique index on <see cref="EventRecord.AggregateTypeUniqueKey"/>.
    ///        Violation of this index must produce <see cref="AggregatePrimaryKeyViolationException"/>.
    /// 
    ///     4. It has to be a non-unique index on <see cref="EventRecord.EventTimestamp"/> to accommodate time related events lookup 
    /// 
    /// ProcessTable Constrains:
    ///     1. it can be only one Aggregate instance per Process (index: <see cref="ProcessRecord.InvolvedAggregateId"/> and <see cref="ProcessRecord.ProcessId"/>"/>
    ///     2. Aggregate can participate only in one process of any given type (index: <see cref="ProcessRecord.InvolvedAggregateId"/> and <see cref="ProcessRecord.ProcessTypeId"/>/>
    /// NOTE: When process is stored, record gets created for each unique combination of ProcessId and AggregateId!!!, 
    /// Storage type specific persistence 
    /// TODO: 
    ///     1) GetPendingDispatch() list of pending dispatch batches, (can  be also be cached in memory for optimization purposes)
    ///     2) Dispatched(commitId) remove dispatch from pending dispatches
    ///     3) GetProcessState(processId) throw Process is complete (non-fatal exception)
    /// 
    /// </summary>
    public interface IPersistenceEngine
    {
        /// <summary>
        /// Initializes the storage. It should create DB structure when needed.
        /// </summary>
        void InitializeStorage();

        /// <summary>
        ///     Completely DESTROYS the contents and schema (if applicable) containing ANY and ALL aggregates that have been
        ///     successfully persisted.  Use with caution.
        /// </summary>
        void DestroyStorage();

        /// <summary>
        /// Restores the state. Prepares persistence engine for work
        /// </summary>
        void RestoreState();

        /// <summary>
        /// Current store version.
        /// </summary>
        long StoreVersion { get; }

        /// <summary>
        /// Version of the store for which all events were dispatched.
        /// </summary>
        /// <value>
        /// The dispatched store version.
        /// </value>
        long DispatchedStoreVersion { get; }

        /// <summary>
        /// Saves the specified events and snapshots.
        /// NOTES:
        /// if DB commit fails due to concurrency violation, commit should re-try until success.
        /// </summary>
        /// <param name="events">The events.</param>
        /// <param name="snapshots">The snapshots.</param>
        /// <param name="processes">The processes.</param>
        /// <param name="processIdsToBeDeleted">The process ids to be deleted. because they completed.
        /// IMPORTANT: engine implementation must NOT fail if there are NO records for specified process id.</param>
        /// <param name="primaryKeyChanges">The primary key changes.</param>
        /// <returns>
        /// Commit Final Store Version
        /// </returns>
        long Commit(IList<EventRecord> events,
            IList<SnapshotRecord> snapshots = null,
            IList<ProcessRecord> processes = null,
            HashSet<Guid> processIdsToBeDeleted = null,
            IList<AggregatePrimaryKeyRecord> primaryKeyChanges = null);

        /// <summary>
        /// Serializes the payload.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns></returns>
        object SerializePayload(object payload);

        /// <summary>
        /// De-serializes the payload.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="type">The type of de-serialized object.</param>
        /// <returns></returns>
        object DeserializePayload(object payload, Type type);

        /// <summary>
        /// Get the events for given aggregate
        /// </summary>
        /// <param name="aggregateId">The aggregate (aggregate) identifier.</param>
        /// <param name="fromAggregateVersion">Event From version. (inclusive)</param>
        /// <param name="toAggregateVersion">Optional. Event To version. (inclusive)</param>
        /// <returns></returns>
        IEnumerable<EventRecord> GetEvents(Guid aggregateId, long fromAggregateVersion, long? toAggregateVersion);

        /// <summary>
        /// Gets the events by time range.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        IEnumerable<EventRecord> GetEvents(DateTimeOffset from, DateTimeOffset? to);

        /// <summary>
        /// Gets the events since commit.
        /// </summary>
        /// <param name="startingStoreVersion">The commit identifier.</param>
        /// <param name="takeEventsCount">The number of events to read. can be null to get up until end of the store</param>
        /// <returns></returns>
        IEnumerable<EventRecord> GetEvents(long startingStoreVersion, long? takeEventsCount);

        /// <summary>
        /// Gets the latest version number. This call may be required to fast check version of any aggregate for validation purposes.
        /// </summary>
        /// <param name="aggregateId">The aggregate (aggregate) identifier.</param>
        /// <returns>Current version of the given aggregate or 0 - if aggregate doesn't exists</returns>
        long GetAggregateVersion(Guid aggregateId);

        /// <summary>
        /// Gets the even version of the aggregate that was snapshot-ed.
        /// </summary>
        /// <param name="aggregateId">The aggregate (aggregate) identifier.</param>
        /// <returns>Most recent version of snapshot or 0 - if aggregate has never been snapshot-ed</returns>
        long GetSnapshotVersion(Guid aggregateId);

        /// <summary>
        /// Loads the latest aggregate snapshot record.
        /// </summary>
        /// <param name="aggregateId">The aggregate(aggregate) identifier.</param>
        /// <param name="throwNotFound">if set to <c>true</c> [throw not found].</param>
        /// <returns></returns>
        /// <exception cref="SnapshotNotFoundException"></exception>
        SnapshotRecord GetSnapshot(Guid aggregateId, bool throwNotFound = true);

        /// <summary>
        /// Gets the process version.
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        /// <returns></returns>
        long GetProcessVersion(Guid processId);
        
        /// <summary>
        /// Gets the process records.
        /// IMPORTANT: record with State (must appear first)
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        /// <returns></returns>
        /// <exception cref="ProcessNotFoundException"></exception>
        IList<ProcessRecord> GetProcessRecords ( Guid processId );

        /// <summary>
        /// Gets the process records.
        /// IMPORTANT: record with State (must appear first)
        /// </summary>
        /// <param name="processTypeId">The process type identifier.</param>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="throwNotFound">if set to <c>true</c> [throw not found].</param>
        /// <returns></returns>
        IList<ProcessRecord> GetProcessRecords(TypeId processTypeId, Guid aggregateId, bool throwNotFound = true);

        /// <summary>
        /// Updates and stores the dispatch version
        /// </summary>
        /// <param name="version">The version.</param>
        void UpdateDispatchVersion(long version);

    }
}