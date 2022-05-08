using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Helpers;
using GhostNetwork.Messages.Chats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Messages.Api.Controllers;

[ApiController]
[Route("chats")]
public class ChatsController : ControllerBase
{
    private readonly IChatsService chatService;
    private readonly IUserProvider userProvider;

    public ChatsController(IChatsService chatService, IUserProvider userProvider)
    {
        this.chatService = chatService;
        this.userProvider = userProvider;
    }

    /// <summary>
    /// Get user's chat
    /// </summary>
    /// <param name="userId">Filters by user</param>
    /// <param name="cursor">Cursor used for pagination</param>
    /// <param name="limit">Limit chats up to a specified position</param>
    /// <response code="200">Exist user chats</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "X-TotalCount", "Number", "Total number of user chats")]
    public async Task<ActionResult<IEnumerable<Chat>>> SearchAsync(
        [FromQuery, Required] Guid userId,
        [FromQuery] string cursor,
        [FromQuery, Range(1, 100)] int limit = 20)
    {
        var filter = new ChatFilter(userId);
        var paging = new Pagination(cursor, limit);

        var chats = await chatService.SearchAsync(filter, paging);

        return Ok(chats);
    }

    /// <summary>
    /// Get chat by identifier
    /// </summary>
    /// <param name="id">Chat identifier</param>
    /// <response code="200">Chat</response>
    /// <response code="404">Chat not fount</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}")]
    public async Task<ActionResult<Chat>> GetByIdAsync([FromRoute] string id)
    {
        var entity = await chatService.GetByIdAsync(new Id(id));

        if (entity is null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    /// <summary>
    /// Create new chat
    /// </summary>
    /// <param name="model">Chat model</param>
    /// <response code="201">Connection successfully created</response>
    /// <response code="400">Problem details</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost]
    public async Task<ActionResult<Chat>> CreateNewChatAsync([FromBody] CreateChatModel model)
    {
        if (model.Participants == null || model.Participants.Count == 0)
        {
            return BadRequest(new ProblemDetails { Title = "Chat should have at least one participant" });
        }

        var participants = await userProvider.SearchAsync(model.Participants);
        if (participants.Count != model.Participants.Count)
        {
            var invalidParticipants = model.Participants.Where(x => participants.All(p => p.Id != x)).ToList();
            return BadRequest(new ProblemDetails { Title = $"Participants {string.Join(", ", invalidParticipants)} is not found" });
        }

        var (result, chat) = await chatService.CreateAsync(model.Name, participants);

        if (result.Successed)
        {
            return Created(Url.Action("GetById", new { chat.Id }) ?? string.Empty, chat);
        }

        return BadRequest(result.ToProblemDetails());
    }

    /// <summary>
    /// Update chat
    /// </summary>
    /// <param name="id">Chat id</param>
    /// <param name="model">Update chat model</param>
    /// <response code="204">Chat successfully updated</response>
    /// <response code="400">Problem details</response>
    /// <response code="404">Chat is not found</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAsync([FromRoute] string id, [FromBody] UpdateChatModel model)
    {
        var chat = await chatService.GetByIdAsync(new Id(id));
        if (chat is null)
        {
            return NotFound();
        }

        if (model.Participants == null || model.Participants.Count == 0)
        {
            return BadRequest(new ProblemDetails { Title = "Chat should have at least one participant" });
        }

        var participants = await userProvider.SearchAsync(model.Participants);
        if (participants.Count != model.Participants.Count)
        {
            var invalidParticipants = model.Participants.Where(x => participants.All(p => p.Id != x)).ToList();
            return BadRequest(new ProblemDetails { Title = $"Participants {string.Join(", ", invalidParticipants)} is not found" });
        }

        chat.Update(model.Name, participants);
        var result = await chatService.UpdateAsync(chat);

        if (!result.Successed)
        {
            return BadRequest(result.ToProblemDetails());
        }

        return NoContent();
    }

    /// <summary>
    /// Delete chat
    /// </summary>
    /// <param name="id">Chat id</param>
    /// <response code="204">Chat successfully deleted</response>
    /// <response code="404">Chat is not found</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteChatAsync([FromRoute] string id)
    {
        var chat = await chatService.GetByIdAsync(new Id(id));

        if (chat is null)
        {
            return NotFound();
        }

        await chatService.DeleteAsync(new Id(id));

        return NoContent();
    }
}

public record CreateChatModel([Required] string Name, List<Guid> Participants);

public record UpdateChatModel([Required] string Name, List<Guid> Participants);