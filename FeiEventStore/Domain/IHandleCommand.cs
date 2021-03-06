﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeiEventStore.Core;

namespace FeiEventStore.Domain
{
    public interface IHandleCommand { }

    public interface IHandleCommand<in TCommand> : IHandleCommand where TCommand : ICommand { }

    public interface IHandleCommand<in TCommand, in TAggregate> : IHandleCommand<TCommand>
        where TCommand : ICommand
        where TAggregate : IAggregate
    {
        /// <summary>
        /// Handles the specified command.
        /// IMPORTANT: before renaming method name search code for .AsDynamic().HandleCommand!!!!
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="aggregate">The aggregate.</param>
        void HandleCommand(TCommand cmd, TAggregate aggregate);
    }
}
