using System;

namespace GhostNetwork.Messages.Messages;

public class Message
{
    public Message(Id id, Id chatId, UserInfo author, DateTimeOffset sentOn, DateTimeOffset updatedOn, string content)
    {
        Id = id;
        ChatId = chatId;
        Author = author;
        SentOn = sentOn;
        UpdatedOn = updatedOn;
        Content = content;
    }

    public Id Id { get; }

    public Id ChatId { get; }

    public UserInfo Author { get; }

    public DateTimeOffset SentOn { get; }

    public DateTimeOffset UpdatedOn { get; private set; }

    public string Content { get; private set; }

    public static Message NewMessage(Id id, Id chatId, UserInfo author, string content)
    {
        var now = DateTimeOffset.UtcNow;

        return new Message(id, chatId, author, now, now, content);
    }

    public Message Update(string content)
    {
        UpdatedOn = DateTimeOffset.UtcNow;
        Content = content;

        return this;
    }
}