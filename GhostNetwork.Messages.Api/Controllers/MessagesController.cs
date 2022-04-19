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
    /// <response code="200">Chat messages</response>
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
    /// Send new message
    /// </summary>
    /// <param name="chatId">Chat identifier</param>
    /// <param name="model">message model</param>
    /// <response code="200">New message</response>
    /// <response code="400">Problem details</response>
    [HttpPost("{chatId:guid}/messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SendAsync(
        [FromRoute] Guid chatId,
        [FromBody] CreateMessageModel model)
    {
        var author = await userProvider.GetByIdAsync(model.SenderId);
        var (result, message) = await messageService.SendAsync(chatId, author, model.Message);

        if (!result.Successed)
        {
            return BadRequest(result.ToProblemDetails());
        }

        return Ok(message);
    }

    /// <summary>
    /// Update message
    /// </summary>
    /// <param name="messageId">Message id</param>
    /// <param name="model">Updated model</param>
    /// <response code="204">Successfully updated</response>
    /// <response code="400">Problem details</response>
    [HttpPut("messages/{messageId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateMessageAsync(
        [FromRoute] string messageId,
        [FromBody] UpdateMessageModel model)
    {
        var message = await messageService.GetByIdAsync(messageId);

        if (message.Author.Id != model.SenderId)
        {
            return BadRequest();
        }

        var result = await messageService.UpdateAsync(messageId, model.Message);

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
    [HttpDelete("{chatId:guid}/messages/{messageId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteMessageAsync(
        [FromRoute] string messageId)
    {
        await messageService.DeleteAsync(messageId);

        return NoContent();
    }
}

public record CreateMessageModel(string SenderId, string Message);

public record UpdateMessageModel(Guid SenderId, string Message);
