using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain;
using GhostNetwork.Messages.Chats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace GhostNetwork.Messages.Api.Handlers.Messages;

public static class CreateHandler
{
    public static async Task<IResult> HandleAsync(
        [FromServices] IChatsStorage chatsStorage,
        [FromServices] IMessagesStorage messagesStorage,
        [FromRoute] string chatId,
        [FromBody, Required] CreateMessageModel model)
    {
        if (string.IsNullOrEmpty(model.Content))
        {
            return Results.BadRequest(new ProblemDetails { Title = "Content is required" });
        }

        if (model.Content.Length > 500)
        {
            return Results.BadRequest(new ProblemDetails { Title = "Content is too long" });
        }

        var chat = await chatsStorage.GetByIdAsync(chatId);
        if (chat == null)
        {
            return Results.NotFound();
        }

        var author = chat.Participants.FirstOrDefault(p => p.Id == model.SenderId);
        if (author == null)
        {
            return Results.BadRequest(new ProblemDetails { Title = "Sender is not in chat" });
        }

        var now = DateTimeOffset.UtcNow;
        var message = new Message(ObjectId.GenerateNewId().ToString(), chat.Id, author, now, now, model.Content);
        await messagesStorage.InsertAsync(message);

        await chatsStorage.ReorderAsync(chat.Id);
        return Results.Created($"/chats/{chat.Id}/messages/{message.Id}", message.Id);
    }
}

public record CreateMessageModel(Guid SenderId, string Content);
