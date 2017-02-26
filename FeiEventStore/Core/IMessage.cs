﻿namespace FeiEventStore.Core
{
    using System;

    public interface IMessage
    {
        MessageOrigin Origin { get; set; }

        Guid? ProcessId { get; set; }
    }
}