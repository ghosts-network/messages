using System.Threading.Tasks;
using GhostNetwork.Messages.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace GhostNetwork.Messages.Api.Handlers.Messages;

public static class GetByIdHandler
{
    public static async Task<IResult> HandleAsync(
        [FromServices] IMessagesStorage messagesStorage,
        [FromRoute] string chatId,
        [FromRoute] string messageId)
    {
        if (!ObjectId.TryParse(chatId, out _))
        {
            return Results.NotFound();
        }

        var message = await messagesStorage.GetByIdAsync(chatId, messageId);

        return message is null
            ? Results.NotFound()
            : Results.Ok(message);
    }
}