﻿namespace FeiEventStore.Events
{
    using System;
    using System.Collections.Generic;
    using Core;
    using Persistence;

    public interface IEventStore
    {
        /// <summary>
        /// Returns most recent store version.
        /// </summary>
        long StoreVersion { get; }


        /// <summary>
        /// Version of the store for which all events were dispatched.
        /// </summary>
        long DispatchedStoreVersion { get; }

        /// <summary>
        /// Saves/Commit the specified events and snapshots
        /// </summary>
        /// <param name="events">The events.</param>
        /// <param name="aggregateConstraints">The constraints.</param>
        /// <param name="snapshots">The snapshots.</param>
        /// <param name="processes">The processes.</param>
        void Commit(IList<IEvent> events,
            IList<Constraint> aggregateConstraints = null,
            IList<SnapshotRecord> snapshots = null,
            IList<IProcess> processes = null);

        /// <summary>
        /// Get the events for given aggregate. 
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <param name="fromAggregateVersion">Event From version. (inclusive)</param>
        /// <param name="toAggregateVersion">Optional. To version. (inclusive)</param>
        /// <returns></returns>
        /// <exception cref="RuntimeTypeInstancesNotFoundException"></exception>
        /// <exception cref="MultipleTypeInstancesException"></exception>
        IList<IEvent> GetEvents(Guid aggregateId, long fromAggregateVersion, long? toAggregateVersion = null);

        /// <summary>
        /// Gets the events since commit.
        /// </summary>
        /// <param name="startingStoreVersion">The commit identifier.</param>
        /// <param name="takeEventsCount">The number of events to read. can be null to get up until end of the store</param>
        /// <returns></returns>
        /// <exception cref="RuntimeTypeInstancesNotFoundException"></exception>
        /// <exception cref="MultipleTypeInstancesException"></exception>
        IList<IEvent> GetEventsSinceStoreVersion(long startingStoreVersion, long? takeEventsCount = null);

        /// <summary>
        /// Gets the aggregate latest version number. This call may be required to fast check version of any aggregate for validation purposes.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <returns>Current version of the given aggregate</returns>
        long GetAggregateVersion(Guid aggregateId);

        /// <summary>
        /// Gets the latest snapshot-ed version of the aggregate.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <returns></returns>
        long GetSnapshotVersion(Guid aggregateId);

        /// <summary>
        /// Gets latest/persisted process version.
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        /// <returns></returns>
        long GetProcessVersion(Guid processId);

        /// <summary>
        /// Loads the latest aggregate. Create new instance of aggregate if given Id doesn't exists
        /// </summary>
        /// <param name="aggregateStateType">Type of the aggregate.</param>
        /// <param name="aggregateId">The aggregate identifier.</param>
        /// <returns></returns>
        /// <exception cref="RuntimeTypeInstancesNotFoundException"></exception>
        /// <exception cref="MultipleTypeInstancesException"></exception>
        IAggregate LoadAggregate(Type aggregateStateType, Guid aggregateId);

        IProcess LoadProcess(Type processStateType, Guid aggregateId );

        IProcess LoadProcess(Guid processId);
    }
}