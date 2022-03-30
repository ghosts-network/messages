using System;

namespace GhostNetwork.Messages
{
    public class Message
    {
        public Message(string id, Guid chatId, Guid senderId, DateTimeOffset sentOn, string data)
        {
            Id = id;
            ChatId = chatId;
            SenderId = senderId;
            SentOn = sentOn;
            Data = data;
        }

        public string Id { get; private set; }

        public Guid ChatId { get; private set; }

        public Guid SenderId { get; private set; }

        public DateTimeOffset SentOn { get; private set; }

        public string Data { get; private set; }

        public static Message NewMessage(Guid chatId, Guid senderId, string data)
        {
            var sentOn = DateTimeOffset.UtcNow;

            return new Message(default, chatId, senderId, sentOn, data);
        }

        // public Message Update(string data)
        // {
        //     Data = data;
        //     UpdatedOn = DateTimeOffset.UtcNow;
        //
        //     return this;
        // }
    }
}