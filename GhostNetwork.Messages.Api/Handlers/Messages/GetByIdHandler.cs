using System;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Messages.Api.Handlers.Messages;

public static class GetByIdHandler
{
    public static async Task<IResult> HandleAsync(
        [FromServices] IMessagesStorage messagesStorage,
        [FromRoute] string chatId,
        [FromRoute] string messageId)
    {
        var message = await messagesStorage.GetByIdAsync(chatId, messageId);

        return message is null
            ? Results.NotFound()
            : Results.Ok(message);
    }
}