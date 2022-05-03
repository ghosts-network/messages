using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;
using GhostNetwork.Messages.Chats;

namespace GhostNetwork.Messages.Messages;

public class MessageValidator : IValidator<MessageContext>
{
    public DomainResult Validate(MessageContext param)
    {
        var resul = Validate(param.Message, param.AuthorId, param.Participants);

        return resul;
    }

    private DomainResult Validate(string message, Guid authorId, IEnumerable<Guid> participants)
    {
        var results = new List<DomainError>();

        if (message == null || message.Length > 500 || string.IsNullOrEmpty(message))
        {
            results.Add(new DomainError($"{nameof(message)} can not be null, empty or more than 500 chars"));
        }

        if (participants.All(x => x != authorId))
        {
            results.Add(new DomainError("You are not a member of this chat!"));
        }

        return !results.Any() ? DomainResult.Success() : DomainResult.Error(results);
    }

    public Task<DomainResult> ValidateAsync(MessageContext context)
    {
        return Task.FromResult(Validate(context));
    }
}