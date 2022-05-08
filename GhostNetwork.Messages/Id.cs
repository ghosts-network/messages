namespace GhostNetwork.Messages;

public record Id(string Value)
{
    public override string ToString()
    {
        return Value;
    }
}
