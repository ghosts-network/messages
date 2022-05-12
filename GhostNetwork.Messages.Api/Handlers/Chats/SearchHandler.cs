using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Chats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Messages.Api.Handlers.Chats;

public static class SearchHandler
{
    public static async Task<IResult> HandleAsync(
        HttpResponse response,
        [FromServices] IChatsStorage chatsStorage,
        [FromQuery, Required] Guid userId,
        [FromQuery] string cursor,
        [FromQuery, Range(1, 100)] int limit = 20)
    {
        var filter = new Filter(userId);
        var paging = new Pagination(cursor, limit);

        var chats = await chatsStorage.SearchAsync(filter, paging);

        if (chats.Any())
        {
            response.Headers.Add("X-Cursor", chats.Last().Id);
        }

        return Results.Ok(chats);
    }
}