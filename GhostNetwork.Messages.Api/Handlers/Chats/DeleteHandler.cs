using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain;
using GhostNetwork.Messages.Chats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Messages.Api.Handlers.Chats;

public static class DeleteHandler
{
    public static async Task<IResult> HandleAsync(
        [FromServices] IChatsStorage chatsStorage,
        [FromServices] IMessagesStorage messagesStorage,
        [FromRoute] string id)
    {
        if (!await chatsStorage.DeleteAsync(id))
        {
            return Results.NotFound();
        }

        await messagesStorage.DeleteByChatAsync(id);

        return Results.NoContent();
    }
}