using System;

namespace GhostNetwork.Messages.Messages
{
    public class Message
    {
        public Message(string id, Guid chatId, UserInfo author, DateTimeOffset sentOn, bool isUpdated, string data)
        {
            Id = id;
            ChatId = chatId;
            Author = author;
            SentOn = sentOn;
            IsUpdated = isUpdated;
            Data = data;
        }

        public string Id { get; }

        public Guid ChatId { get; }

        public UserInfo Author { get; }

        public DateTimeOffset SentOn { get; private set; }

        public bool IsUpdated { get; private set; }

        public string Data { get; private set; }

        public static Message NewMessage(Guid chatId, UserInfo author, string data)
        {
            var sentOn = DateTimeOffset.UtcNow;

            return new Message(default, chatId, author, sentOn, false, data);
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