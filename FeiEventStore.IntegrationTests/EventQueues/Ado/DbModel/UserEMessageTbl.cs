﻿using System;

namespace FeiEventStore.IntegrationTests.EventQueues.Ado.DbModel
{
    public class UserEMessageTbl
    {
        public UserEMessageTbl()
        {
        }
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid EMessageId { get; set; }

        public bool IsRead { get; set; }

        public bool Flagged { get; set; }

        public string FolderTag { get; set; } //inbox, deleted, etc

    }
}