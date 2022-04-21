using System;
using System.Collections.Generic;

namespace GhostNetwork.Messages.Chats;

public class Chat
{
    public Chat(Guid id, string name, IEnumerable<UserInfo> participants)
    {
        Id = id;
        Name = name;
        Participants = participants;
    }

    public Guid Id { get; }

    public string Name { get; private set; }

    public IEnumerable<UserInfo> Participants { get; private set; }

    public static Chat NewChat(string name, IEnumerable<UserInfo> participants)
    {
        var id = Guid.NewGuid();

        return new Chat(id, name, participants);
    }

    public Chat Update(string name, IEnumerable<UserInfo> participants)
    {
        Name = name;
        Participants = participants;

        return this;
    }
}