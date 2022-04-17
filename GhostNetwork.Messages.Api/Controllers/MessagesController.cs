using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Messages.Api.Controllers;

[ApiController]
[Route("chats/{chatId:guid}/messages")]
public class MessagesController : ControllerBase
{
    private readonly IMessagesService messageService;

    public MessagesController(IMessagesService messageService)
    {
        this.messageService = messageService;
    }

    /// <summary>
    /// Get messages by chat id
    /// </summary>
    /// <param name="chatId">Chat identifier</param>
    /// <param name="skip">Skip exist messages up to a specified position</param>
    /// <param name="take">Take exist messages up to a specified position</param>
    /// <response code="200">Chat messages</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "X-TotalCount", "Number", "Total number of messages")]
    public async Task<ActionResult<IEnumerable<Message>>> SearchAsync(
        [FromRoute] Guid chatId,
        [FromQuery, Range(0, int.MaxValue)] int skip,
        [FromQuery, Range(1, 100)] int take)
    {
        var (messages, totalCount) = await messageService.SearchAsync(skip, take, chatId);

        Response.Headers.Add("X-TotalCount", totalCount.ToString());

        return Ok(messages);
    }

    /// <summary>
    /// Send new message
    /// </summary>
    /// <param name="chatId">Chat identifier</param>
    /// <param name="model">message model</param>
    /// <response code="200">New message</response>
    /// <response code="400">Problem details</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SendAsync(
        [FromRoute] Guid chatId,
        [FromBody] CreateMessageModel model)
    {
        var (result, message) = await messageService.SendAsync(chatId, model.SenderId, model.Message);

        if (!result.Successed)
        {
            return BadRequest(result.Errors);
        }

        return Ok(message);
    }

    /// <summary>
    /// Update message
    /// </summary>
    /// <param name="id">Message id</param>
    /// <param name="model">Updated model</param>
    /// <response code="204">Successfully updated</response>
    /// <response code="400">Problem details</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateMessageModel model)
    {
        var result = await messageService.UpdateAsync(id, model.Message);

        if (!result.Successed)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    /// <summary>
    /// Delete message
    /// </summary>
    /// <param name="id">Message identifier</param>
    /// <response code="204">Message successfully deleted</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id)
    {
        await messageService.DeleteAsync(id);

        return NoContent();
    }
}

public record CreateMessageModel(Guid SenderId, string Message);

public record UpdateMessageModel(string Message);
