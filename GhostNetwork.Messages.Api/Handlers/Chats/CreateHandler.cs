using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Chats;
using GhostNetwork.Messages.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace GhostNetwork.Messages.Api.Handlers.Chats;

public static class CreateHandler
{
    public static async Task<IResult> HandleAsync(
        [FromServices] IUsersStorage usersStorage,
        [FromServices] IChatsStorage chatsStorage,
        [FromBody] CreateChatModel model)
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

        var participants = await usersStorage.SearchAsync(model.Participants);
        if (participants.Count != model.Participants.Count)
        {
            var invalidParticipants = model.Participants.Where(x => participants.All(p => p.Id != x)).ToList();
            return Results.BadRequest(new ProblemDetails
            {
                Title = $"Participants {string.Join(", ", invalidParticipants)} is not found"
            });
        }

        var chat = new Chat(ObjectId.GenerateNewId().ToString(), model.Name, participants);
        await chatsStorage.InsertAsync(chat);

        return Results.Created($"/chats/{chat.Id}", chat);
    }
}

public record CreateChatModel(string Name, List<Guid> Participants);