using System;
using System.Collections;
using System.Collections.Generic;

namespace GhostNetwork.Messages;

public class Chat
{
    public Chat(Guid id, IEnumerable<Guid> usersIds)
    {
        Id = id;
        UsersIds = usersIds;
    }

    public Guid Id { get; }

    public IEnumerable<Guid> UsersIds { get; private set; }

    public static Chat NewChat(IEnumerable<Guid> usersIds)
    {
        var id = Guid.NewGuid();

        return new Chat(id, usersIds);
    }
}