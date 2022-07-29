using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Chats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

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
        if (limit is < 1 or > 100)
        {
            return Results.BadRequest(new ProblemDetails { Title = "Limit must be between 1 and 100" });
        }

        if (!ObjectId.TryParse(cursor, out _))
        {
            cursor = null;
        }

        var filter = new Filter(userId);
        var paging = new Pagination(cursor, limit);

        var (chats, totalCount) = await chatsStorage.SearchAsync(filter, paging);

        if (chats.Any())
        {
            response.Headers.Add("X-Cursor", chats.Last().Id);
            response.Headers.Add("X-TotalCount", totalCount.ToString());
        }

        return Results.Ok(chats);
    }
}