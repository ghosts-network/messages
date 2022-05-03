using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;

namespace GhostNetwork.Messages.Chats;

public class ChatValidator : IValidator<ChatContext>
{
    public DomainResult Validate(ChatContext param)
    {
        var resul = Validate(param.Name, param.Users);

        return resul;
    }

    private DomainResult Validate(string name, List<UserInfo> users)
    {
        var results = new List<DomainError>();

        if (name == null || name.Length > 500 || string.IsNullOrEmpty(name))
        {
            results.Add(new DomainError($"{nameof(name)} can not be null, empty or more than 500 chars"));
        }

        if (users.Count > 500 || !users.Any())
        {
            results.Add(new DomainError($"Chat cannot contain more then 500 users or by empty."));
        }

        var duplicates = users.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key);

        if (duplicates.Any())
        {
            results.Add(new DomainError($"Chat contains duplicate user."));
        }

        return !results.Any() ? DomainResult.Success() : DomainResult.Error(results);
    }

    public Task<DomainResult> ValidateAsync(ChatContext context)
    {
        return Task.FromResult(Validate(context));
    }
}