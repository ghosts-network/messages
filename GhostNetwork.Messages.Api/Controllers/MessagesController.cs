using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Helpers;
using GhostNetwork.Messages.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Messages.Api.Controllers;

[ApiController]
[Route("chats")]
public class MessagesController : ControllerBase
{
    private readonly IMessagesService messageService;
    private readonly IUserProvider userProvider;

    public MessagesController(IMessagesService messageService, IUserProvider userProvider)
    {
        this.messageService = messageService;
        this.userProvider = userProvider;
    }

    /// <summary>
    /// Get messages by chat id
    /// </summary>
    /// <param name="chatId">Chat identifier</param>
    /// <param name="lastMessageId">Last message id for cursor pagination</param>
    /// <param name="take">Take exist messages up to a specified position</param>
    /// <response code="200">Messages</response>
    [HttpGet("{chatId:guid}/messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "X-TotalCount", "Number", "Total number of messages")]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "X-LastMessageId", "String", "Last message id for cursor pagination")]
    public async Task<ActionResult<IEnumerable<Message>>> SearchAsync(
        [FromRoute] Guid chatId,
        [FromQuery] string lastMessageId,
        [FromQuery, Range(1, 100)] int take)
    {
        var (messages, totalCount, lastMessage) = await messageService.SearchAsync(lastMessageId, take, chatId);

        Response.Headers.Add("X-TotalCount", totalCount.ToString());
        Response.Headers.Add("X-LastMessageId", lastMessage);

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
        var message = await messageService.GetByIdAsync(messageId);

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
    [HttpPost("{chatId:guid}/messages")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Message>> SendAsync(
        [FromRoute] Guid chatId,
        [FromBody, Required] CreateMessageModel model)
    {
        var author = await userProvider.GetByIdAsync(model.SenderId);

        if (author is null)
        {
            return NotFound();
        }

        var (result, id) = await messageService.SendAsync(chatId, author, model.Message);

        if (!result.Successed)
        {
            return BadRequest(result.ToProblemDetails());
        }

        return Created(Url.Action("GetById", new { id }) ?? string.Empty, await messageService.GetByIdAsync(id));
    }

    /// <summary>
    /// Update message
    /// </summary>
    /// <param name="chatId">Chat id</param>
    /// <param name="messageId">Message id</param>
    /// <param name="model">Updated model</param>
    /// <response code="204">Successfully updated</response>
    /// <response code="400">Problem details</response>
    /// <response code="404">Message not found</response>
    [HttpPut("{chatId:guid}/messages/{messageId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute] Guid chatId,
        [FromRoute] string messageId,
        [FromBody, Required] UpdateMessageModel model)
    {
        var message = await messageService.GetByIdAsync(messageId);

        if (message is null)
        {
            return NotFound();
        }

        var result = await messageService.UpdateAsync(messageId, chatId, model.Message, model.SenderId);

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
        var message = await messageService.GetByIdAsync(messageId);

        if (message is null)
        {
            return NotFound();
        }

        await messageService.DeleteAsync(messageId);

        return NoContent();
    }
}

public record CreateMessageModel([Required] string SenderId, [Required] string Message);

public record UpdateMessageModel([Required] Guid SenderId, [Required] string Message);
