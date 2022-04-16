using System;
using System.Collections.Generic;

namespace GhostNetwork.Messages.Chats;

public class ChatContext
{
    public ChatContext(string name, List<Guid> users)
    {
        Name = name;
        Users = users;
    }

    public string Name { get; set; }

    public List<Guid> Users { get; set; }
}