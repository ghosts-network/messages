using System;
using System.Collections.Generic;

namespace GhostNetwork.Messages.Messages;

public class MessageContext
{
    public MessageContext(string message, Guid authorId, IEnumerable<Guid> participants)
    {
        Message = message;
        AuthorId = authorId;
        Participants = participants;
    }

    public string Message { get; set; }

    public Guid AuthorId { get; set; }

    public IEnumerable<Guid> Participants { get; set; }
}