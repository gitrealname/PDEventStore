﻿namespace FeiEventStore.Core
{
    using System;

    /// <summary>
    /// Any store-able type (e.g. IState, IPayload) that replaces obsolete type
    /// Must be marked with this interface.
    /// </summary>
    /// <typeparam name="TObsolete">The type of the obsolete object.</typeparam>
    public interface IReplace<in TObsolete>  where TObsolete : IPermanentlyTyped, IState
    {
        /// <summary>
        /// Initializes from obsolete object type.
        /// </summary>
        /// <param name="obsoleteObject">The obsolete object.</param>
        void InitFromObsolete(TObsolete obsoleteObject);
    }
}
