using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain;
using GhostNetwork.Messages.Chats;
using GhostNetwork.Messages.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Filters;
using Filter = GhostNetwork.Messages.Chats.Filter;

namespace GhostNetwork.Messages.Api.Controllers;

[ApiController]
[Route("chats")]
public class ChatsController : ControllerBase
{
    private readonly IChatsStorage chatsStorage;
    private readonly IMessagesStorage messagesStorage;
    private readonly IUsersStorage usersStorage;

    public ChatsController(
        IChatsStorage chatsStorage,
        IMessagesStorage messagesStorage,
        IUsersStorage usersStorage)
    {
        this.chatsStorage = chatsStorage;
        this.messagesStorage = messagesStorage;
        this.usersStorage = usersStorage;
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
        if (!ObjectId.TryParse(id, out var objectId))
        {
            return NotFound();
        }

        var entity = await chatsStorage.GetByIdAsync(objectId);

        if (entity is null)
        {
            return NotFound();
        }

        return Ok(entity);
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
        var filter = new Filter(userId);
        var paging = new Pagination(cursor, limit);

        var chats = await chatsStorage.SearchAsync(filter, paging);

        return Ok(chats);
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
    public async Task<ActionResult<Chat>> CreateAsync([FromBody] CreateChatModel model)
    {
        var participants = await usersStorage.SearchAsync(model.Participants);
        if (participants.Count != model.Participants.Count)
        {
            var invalidParticipants = model.Participants.Where(x => participants.All(p => p.Id != x)).ToList();
            return BadRequest(new ProblemDetails
            {
                Title = $"Participants {string.Join(", ", invalidParticipants)} is not found"
            });
        }

        var chat = new Chat(ObjectId.GenerateNewId(), model.Name, participants);
        await chatsStorage.InsertAsync(chat);

        return Created(Url.Action("GetById", new { chat.Id }) ?? string.Empty, chat);
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
        if (!ObjectId.TryParse(id, out var objectId))
        {
            return NotFound();
        }

        var chat = await chatsStorage.GetByIdAsync(objectId);
        if (chat is null)
        {
            return NotFound();
        }

        var participants = await usersStorage.SearchAsync(model.Participants);
        if (participants.Count != model.Participants.Count)
        {
            var invalidParticipants = model.Participants.Where(x => participants.All(p => p.Id != x)).ToList();
            return BadRequest(new ProblemDetails { Title = $"Participants {string.Join(", ", invalidParticipants)} is not found" });
        }

        chat = chat with { Name = model.Name, Participants = participants };
        await chatsStorage.UpdateAsync(chat);

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
    public async Task<ActionResult> DeleteAsync([FromRoute] string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            return NotFound();
        }

        if (!await chatsStorage.DeleteAsync(objectId))
        {
            return NotFound();
        }

        await messagesStorage.DeleteByChatAsync(objectId);

        return NoContent();
    }
}

public record CreateChatModel(
    [Required, StringLength(50)] string Name,
    [Required, MinLength(2), MaxLength(20)] List<Guid> Participants);

public record UpdateChatModel(
    [Required, StringLength(50)] string Name,
    [Required, MinLength(2), MaxLength(20)] List<Guid> Participants);