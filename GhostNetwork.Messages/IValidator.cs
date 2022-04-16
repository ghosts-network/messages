using Domain;

namespace GhostNetwork.Messages;

public interface IValidator<in T>
{
    DomainResult Validate(T param);
}