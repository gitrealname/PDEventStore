using System;
using FeiEventStore.Core;

namespace EventStoreIntegrationTester.Domain.UserEMessage.Messages
{
    [PermanentType("user.message.created")]
    public class UserEMessageCreated : IEvent
    {
        public Guid MessageId { get; set; }

        public Guid UserId { get; set; }

        public string FolderTag { get; set; }

    }
}