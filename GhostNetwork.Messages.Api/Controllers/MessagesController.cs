using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Helpers;
using GhostNetwork.Messages.Chats;
using GhostNetwork.Messages.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Messages.Api.Controllers;

[ApiController]
[Route("chats")]
public class MessagesController : ControllerBase
{
    private readonly IMessagesService messageService;
    private readonly IChatsService chatsService;
    private readonly IUserProvider userProvider;

    public MessagesController(IMessagesService messageService, IChatsService chatsService, IUserProvider userProvider)
    {
        this.messageService = messageService;
        this.chatsService = chatsService;
        this.userProvider = userProvider;
    }

    /// <summary>
    /// Get messages by chat id
    /// </summary>
    /// <param name="chatId">Chat identifier</param>
    /// <param name="cursor">Cursor used for pagination</param>
    /// <param name="limit">Take exist messages up to a specified position</param>
    /// <response code="200">Messages</response>
    [HttpGet("{chatId}/messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Message>>> SearchAsync(
        [FromRoute] string chatId,
        [FromQuery] string cursor,
        [FromQuery, Range(1, 100)] int limit = 20)
    {
        var filter = new MessageFilter(new Id(chatId));
        var paging = new Pagination(cursor, limit);

        var messages = await messageService.SearchAsync(filter, paging);

        return Ok(messages);
    }

    /// <summary>
    /// Get message by id
    /// </summary>
    /// <param name="messageId">Message id</param>
    /// <response code="200">Message</response>
    /// <response code="404">Message is not found</response>
    [HttpGet("messages/{messageId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Message>> GetByIdAsync([FromRoute] string messageId)
    {
        var message = await messageService.GetByIdAsync(new Id(messageId));

        if (message is null)
        {
            return NotFound();
        }

        return Ok(message);
    }

    /// <summary>
    /// Send new message
    /// </summary>
    /// <param name="chatId">Chat identifier</param>
    /// <param name="model">message model</param>
    /// <response code="201">New message</response>
    /// <response code="400">Problem details</response>
    /// <response code="400">Author profile or chat is not found</response>
    [HttpPost("{chatId}/messages")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Message>> SendAsync(
        [FromRoute] string chatId,
        [FromBody, Required] CreateMessageModel model)
    {
        var chatIdentifier = new Id(chatId);
        if (await chatsService.GetByIdAsync(chatIdentifier) == null)
        {
            return NotFound();
        }

        var author = await userProvider.GetByIdAsync(model.SenderId);

        if (author is null)
        {
            return BadRequest(new ProblemDetails { Title = "Author is not found" });
        }

        var (result, id) = await messageService.SendAsync(chatIdentifier, author, model.Message);

        if (!result.Successed)
        {
            return BadRequest(result.ToProblemDetails());
        }

        await chatsService.ReorderAsync(chatIdentifier);
        return Created(Url.Action("GetById", new { id }) ?? string.Empty, await messageService.GetByIdAsync(id));
    }

    /// <summary>
    /// Update message
    /// </summary>
    /// <param name="messageId">Message id</param>
    /// <param name="model">Updated model</param>
    /// <response code="204">Successfully updated</response>
    /// <response code="400">Problem details</response>
    /// <response code="404">Message not found</response>
    [HttpPut("{chatId}/messages/{messageId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute] string messageId,
        [FromBody, Required] UpdateMessageModel model)
    {
        var messageIdentifier = new Id(messageId);
        var message = await messageService.GetByIdAsync(messageIdentifier);

        if (message is null)
        {
            return NotFound();
        }

        var result = await messageService.UpdateAsync(messageIdentifier, model.Message, model.SenderId);

        if (!result.Successed)
        {
            return BadRequest(result.ToProblemDetails());
        }

        return NoContent();
    }

    /// <summary>
    /// Delete message
    /// </summary>
    /// <param name="messageId">Message identifier</param>
    /// <response code="204">Message successfully deleted</response>
    /// <response code="404">Message is not found</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("messages/{messageId}")]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] string messageId)
    {
        var messageIdentifier = new Id(messageId);
        var message = await messageService.GetByIdAsync(messageIdentifier);

        if (message is null)
        {
            return NotFound();
        }

        await messageService.DeleteAsync(messageIdentifier);

        return NoContent();
    }
}

public record CreateMessageModel([Required] Guid SenderId, [Required] string Message);

public record UpdateMessageModel([Required] Guid SenderId, [Required] string Message);
