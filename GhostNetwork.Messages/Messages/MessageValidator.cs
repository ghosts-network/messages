using System.Collections.Generic;
using System.Linq;
using Domain;

namespace GhostNetwork.Messages.Messages;

public class MessageValidator : IValidator<MessageContext>
{
    public DomainResult Validate(MessageContext param)
    {
        var resul = Validate(param.Message);

        return resul;
    }

    private DomainResult Validate(string message)
    {
        var results = new List<DomainError>();

        if (message == null || message.Length > 500 || string.IsNullOrEmpty(message))
        {
            results.Add(new DomainError($"{nameof(message)} can not be null, empty or more than 500 chars"));
        }

        return !results.Any() ? DomainResult.Success() : DomainResult.Error(results);
    }
}