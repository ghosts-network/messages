using System;
using System.Collections.Generic;

namespace GhostNetwork.Messages.Chats;

public class Chat
{
    public Chat(Guid id, string name, List<Guid> users)
    {
        Id = id;
        Name = name;
        Users = users;
    }

    public Guid Id { get; }

    public string Name { get; private set; }

    public List<Guid> Users { get; private set; }

    public static Chat NewChat(string name, List<Guid> users)
    {
        var id = Guid.NewGuid();

        return new Chat(id, name, users);
    }

    public Chat Update(string name, List<Guid> users)
    {
        Name = name;
        Users = users;

        return this;
    }
}