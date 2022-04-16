namespace GhostNetwork.Messages;

public class MessageContext
{
    public MessageContext(string message)
    {
        Message = message;
    }

    public string Message { get; set; }
}