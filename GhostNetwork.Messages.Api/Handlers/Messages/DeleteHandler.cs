using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain.Messages;
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
        if (!ObjectId.TryParse(chatId, out _) || !ObjectId.TryParse(messageId, out _))
        {
            return Results.NotFound();
        }

        return await messagesStorage.DeleteAsync(chatId, messageId)
            ? Results.NoContent()
            : Results.NotFound();
    }
}