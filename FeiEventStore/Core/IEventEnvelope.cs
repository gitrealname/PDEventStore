﻿namespace FeiEventStore.Core
{
    using System;

    public interface IEventEnvelope : IMessage
    {
        string Origin { get; set; }

        long StoreVersion { get; set; }

        Guid AggregateId { get; set; }

        long AggregateVersion { get; set; }

        TypeId AggregateTypeId { get; set; }

        DateTimeOffset Timestamp { get; set; }

        object Payload { get; set; }
    }

    public interface IEventEnvelope<TEvent> : IEventEnvelope where TEvent : IEvent, new ()
    {
        new TEvent Payload { get; set; }
    }
}
