﻿using System;
using System.Collections.Generic;
using System.Threading;
using FeiEventStore.Core;
using FeiEventStore.EventQueue;
using FeiEventStore.Events;

namespace FeiEventStore.IntegrationTests.EventQueues.Printer
{
    [PermanentType("printer.event.queue")]
    public class PrinterTransactionalEventQueue : TransactionalEventQueueBase, ITestingEventQueue
    {
        private readonly IPrinterEventQueueConfiguration _config;

        public PrinterTransactionalEventQueue(IPrinterEventQueueConfiguration config, IEventStore eventStore, IVersionTrackingStore verstionStore, IEventQueueAwaiter queueAwaiter) 
            : base(config, eventStore, verstionStore, queueAwaiter)
        {
            _config = config;
        }

        protected override void HandleEvents(ICollection<IEventEnvelope> events)
        {
            foreach(var e in events)
            {
                var msg = string.Format("({0}) Event type id '{1}' source stream Id '{2}' stream version {3}"
                    , e.StoreVersion, e.Payload.GetType().GetPermanentTypeId(), e.AggregateId, e.AggregateVersion);

                _config.Output.Add(msg);
                if(_config.PrintToConsole)
                {
                    Console.WriteLine("  " + msg);
                }
            }
            //Thread.Sleep(3000);
        }

        public void ResetStoreVersion()
        {
            this._version = 0;
            this._config.Output.Clear();
        }

        protected override void OnBeforeBlocking()
        {
            _config.DoneEvent.Set();
        }

        protected override void OnAfterBlocking()
        {
            _config.DoneEvent.Reset();
        }

        public WaitHandle GetDoneEvent()
        {
            return _config.DoneEvent;
        }


        public void UpdateCancelationToken(CancellationToken token)
        {
            _config.UpdateCancelationToken(token);
        }
    }
}
