using System;

namespace GhostNetwork.Messages;

public class Chat
{
    public Chat(Guid id, Guid senderId, Guid receiverId)
    {
        Id = id;
        SenderId = senderId;
        ReceiverId = receiverId;
    }

    public Guid Id { get; set; }

    public Guid SenderId { get; set; }

    public Guid ReceiverId { get; set; }

    public static Chat NewChat(Guid id, Guid senderId, Guid receiverId)
    {
        return new Chat(id, senderId, receiverId);
    }
}