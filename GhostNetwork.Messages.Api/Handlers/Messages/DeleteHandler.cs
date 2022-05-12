using System.Threading.Tasks;
using GhostNetwork.Messages.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace GhostNetwork.Messages.Api.Handlers.Messages;

public static class DeleteHandler
{
    public static async Task<IResult> HandleAsync(
        [FromServices] IMessagesStorage messagesStorage,
        [FromRoute] string chatId,
        [FromRoute] string messageId)
    {
        if (!ObjectId.TryParse(chatId, out _) || !await messagesStorage.DeleteAsync(chatId, messageId))
        {
            return Results.NotFound();
        }

        return Results.NoContent();
    }
}