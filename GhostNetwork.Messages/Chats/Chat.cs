using System.Collections.Generic;

namespace GhostNetwork.Messages.Chats;

public class Chat
{
    public Chat(Id id, string name, IReadOnlyCollection<UserInfo> participants)
    {
        Id = id;
        Name = name;
        Participants = participants;
    }

    public Id Id { get; }

    public string Name { get; private set; }

    public IReadOnlyCollection<UserInfo> Participants { get; private set; }

    public static Chat NewChat(Id id, string name, IReadOnlyCollection<UserInfo> participants)
    {
        return new Chat(id, name, participants);
    }

    public void Update(string name, IReadOnlyCollection<UserInfo> participants)
    {
        Name = name;
        Participants = participants;
    }
}