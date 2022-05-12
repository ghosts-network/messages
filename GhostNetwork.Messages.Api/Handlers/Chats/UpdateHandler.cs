using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Chats;
using GhostNetwork.Messages.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Messages.Api.Handlers.Chats;

public static class UpdateHandler
{
    public static async Task<IResult> HandleAsync(
        [FromServices] IUsersStorage usersStorage,
        [FromServices] IChatsStorage chatsStorage,
        [FromRoute] string id,
        [FromBody] UpdateChatModel model)
    {
        if (string.IsNullOrEmpty(model.Name))
        {
            return Results.BadRequest(new ProblemDetails { Title = "Content is required" });
        }

        if (model.Name.Length > 50)
        {
            return Results.BadRequest(new ProblemDetails { Title = "Content is too long" });
        }

        if (model.Participants == null || model.Participants.Count == 0)
        {
            return Results.BadRequest(new ProblemDetails { Title = "Participants are required" });
        }

        var chat = await chatsStorage.GetByIdAsync(id);
        if (chat is null)
        {
            return Results.NotFound();
        }

        var participants = await usersStorage.SearchAsync(model.Participants);
        if (participants.Count != model.Participants.Count)
        {
            var invalidParticipants = model.Participants.Where(x => participants.All(p => p.Id != x)).ToList();
            return Results.BadRequest(new ProblemDetails { Title = $"Participants {string.Join(", ", invalidParticipants)} is not found" });
        }

        chat = chat with { Name = model.Name, Participants = participants };
        await chatsStorage.UpdateAsync(chat);

        return Results.NoContent();
    }
}

public record UpdateChatModel(string Name, List<Guid> Participants);