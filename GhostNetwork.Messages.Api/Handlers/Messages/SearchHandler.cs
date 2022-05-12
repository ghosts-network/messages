using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Messages.Api.Handlers.Messages;

public static class SearchHandler
{
    public static async Task<IResult> HandleAsync(
        [FromServices] HttpResponse response,
        [FromServices] IMessagesStorage messagesStorage,
        [FromRoute] string chatId,
        [FromQuery] string cursor,
        [FromQuery, Range(1, 100)] int limit)
    {
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