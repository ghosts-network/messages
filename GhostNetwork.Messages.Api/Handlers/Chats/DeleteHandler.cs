using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain.Chats;
using GhostNetwork.Messages.Api.Domain.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace GhostNetwork.Messages.Api.Handlers.Chats;

public static class DeleteHandler
{
    public static async Task<IResult> HandleAsync(
        [FromServices] IChatsStorage chatsStorage,
        [FromServices] IMessagesStorage messagesStorage,
        [FromRoute] string id)
    {
        if (!ObjectId.TryParse(id, out _) || !await chatsStorage.DeleteAsync(id))
        {
            return Results.NotFound();
        }

        await messagesStorage.DeleteByChatAsync(id);

        return Results.NoContent();
    }
}