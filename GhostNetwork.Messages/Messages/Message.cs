using System;

namespace GhostNetwork.Messages.Messages
{
    public class Message
    {
        public Message(string id, Guid chatId, Guid senderId, DateTimeOffset sentOn, bool isUpdated, string data)
        {
            Id = id;
            ChatId = chatId;
            SenderId = senderId;
            SentOn = sentOn;
            IsUpdated = isUpdated;
            Data = data;
        }

        public string Id { get; }

        public Guid ChatId { get; private set; }

        public Guid SenderId { get; private set; }

        public DateTimeOffset SentOn { get; private set; }

        public bool IsUpdated { get; private set; }

        public string Data { get; private set; }

        public static Message NewMessage(Guid chatId, Guid senderId, string data)
        {
            var sentOn = DateTimeOffset.UtcNow;

            return new Message(default, chatId, senderId, sentOn, false, data);
        }

        public Message Update(string data)
        {
            SentOn = DateTimeOffset.Now;
            IsUpdated = true;
            Data = data;

            return this;
        }
    }
}