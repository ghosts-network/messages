using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Messages.Api.Handlers.Messages;

public static class UpdateHandler
{
    public static async Task<IResult> HandleAsync(
        [FromServices] IMessagesStorage messagesStorage,
        [FromRoute] string chatId,
        [FromRoute] string messageId,
        [FromBody, Required] UpdateMessageModel model)
    {
        if (string.IsNullOrEmpty(model.Content))
        {
            return Results.BadRequest(new ProblemDetails { Title = "Content is required" });
        }

        if (model.Content.Length > 500)
        {
            return Results.BadRequest(new ProblemDetails { Title = "Content is too long" });
        }

        var message = await messagesStorage.GetByIdAsync(chatId, messageId);

        if (message is null)
        {
            return Results.NotFound();
        }

        message = message with { Content = model.Content };
        await messagesStorage.UpdateAsync(message);

        return Results.NoContent();
    }
}

public record UpdateMessageModel(string Content);
