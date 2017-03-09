﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using EventStoreIntegrationTester.Counter.Messages;
using EventStoreIntegrationTester.UserGroup.Messages;
using FeiEventStore.Core;
using FeiEventStore.Domain;

namespace EventStoreIntegrationTester.UserGroup
{
    [PermanentType("user.group.counter.state")]
    public class CreateUserGroupCounterProcessManagerState : IState
    {
        public bool LongRunning { get; set; }
        public int ProcessedEventCount { get; set; }
    }

    [PermanentType("user.group.counter")]
    public class CreateUserGroupCounterProcessManager : BaseProcess<CreateUserGroupCounterProcessManagerState>
        ,IStartedByEvent<UserGroupCreated>
        ,IHandleEvent<Incremented>
    {
        public void StartByEvent(UserGroupCreated @event)
        {
            if(@event.Payload.GroupCounterId != null)
            {
                if(@event.Payload.Name.StartsWith("_"))
                {
                    State.LongRunning = true;
                }
                State.ProcessedEventCount++;
                var increment = new Increment(@event.Payload.GroupCounterId.Value, 1);

                ScheduleCommand(increment);

                IsComplete = false;
            }
        }

        public void HandleEvent(Incremented @event)
        {
            State.ProcessedEventCount++;
            //long running process ends when counter incremented by 100
            if(!State.LongRunning || @event.Payload.By == 100)
            {
                IsComplete = true;
            } else
            {
                IsComplete = false;
            }
        }
    }
}
