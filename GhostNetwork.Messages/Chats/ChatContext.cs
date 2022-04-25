using System;
using System.Collections.Generic;

namespace GhostNetwork.Messages.Chats;

public class ChatContext
{
    public ChatContext(string name, List<UserInfo> users)
    {
        Name = name;
        Users = users;
    }

    public string Name { get; set; }

    public List<UserInfo> Users { get; set; }
}