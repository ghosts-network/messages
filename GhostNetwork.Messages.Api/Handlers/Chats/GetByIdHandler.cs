using System.Threading.Tasks;
using GhostNetwork.Messages.Chats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace GhostNetwork.Messages.Api.Handlers.Chats;

public static class GetByIdHandler
{
    public static async Task<IResult> HandleAsync(
        [FromServices] IChatsStorage chatsStorage,
        [FromRoute] string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return Results.NotFound();
        }

        return await chatsStorage.GetByIdAsync(id)
            is { } chat
            ? Results.Ok(chat)
            : Results.NotFound();
    }
}