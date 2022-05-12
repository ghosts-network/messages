using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Messages.Api.Handlers.Messages;

public static class DeleteHandler
{
    public static async Task<IResult> HandleAsync(
        [FromServices] IMessagesStorage messagesStorage,
        [FromRoute] string chatId,
        [FromRoute] string messageId)
    {
        if (await messagesStorage.DeleteAsync(chatId, messageId))
        {
            return Results.NoContent();
        }

        return Results.NotFound();
    }
}