using System;
using System.Collections.Generic;

namespace GhostNetwork.Messages.Chats;

public class Chat
{
    public Chat(Guid id, string name, IReadOnlyCollection<UserInfo> participants)
    {
        Id = id;
        Name = name;
        Participants = participants;
    }

    public Guid Id { get; }

    public string Name { get; private set; }

    public IReadOnlyCollection<UserInfo> Participants { get; private set; }

    public static Chat NewChat(string name, IReadOnlyCollection<UserInfo> participants)
    {
        var id = Guid.NewGuid();

        return new Chat(id, name, participants);
    }

    public Chat Update(string name, IReadOnlyCollection<UserInfo> participants)
    {
        Name = name;
        Participants = participants;

        return this;
    }
}