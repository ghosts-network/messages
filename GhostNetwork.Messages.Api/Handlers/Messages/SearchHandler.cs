using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Messages.Api.Handlers.Messages;

public static class SearchHandler
{
    public static async Task<IResult> HandleAsync(
        HttpResponse response,
        [FromServices] IMessagesStorage messagesStorage,
        [FromRoute] string chatId,
        [FromQuery] string cursor,
        [FromQuery] int limit = 20)
    {
        if (limit is < 1 or > 100)
        {
            return Results.BadRequest(new ProblemDetails { Title = "Limit must be between 1 and 100" });
        }

        var filter = new Filter(chatId);
        var paging = new Pagination(cursor, limit);

        var messages = await messagesStorage.SearchAsync(filter, paging);

        if (messages.Any())
        {
            response.Headers.Add("X-Cursor", messages.Last().Id);
        }

        return Results.Ok(messages);
    }
}