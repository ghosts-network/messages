using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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

    public ChatsController(IChatsService chatService)
    {
        this.chatService = chatService;
    }

    /// <summary>
    /// Get exist user chats by user id
    /// </summary>
    /// <param name="skip">Skip exist chats up to a specified position</param>
    /// <param name="take">Take exist chats up to a specified position</param>
    /// <param name="userId">Filters by user</param>
    /// <response code="200">Exist chats</response>
    [HttpGet("search/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerResponseHeader(StatusCodes.Status200OK, "X-TotalCount", "Number", "Total number of user chats")]
    public async Task<ActionResult<IEnumerable<Chat>>> SearchExistChatsAsync(
        [FromQuery, Range(0, int.MaxValue)] int skip,
        [FromQuery, Range(1, 100)] int take,
        [FromRoute] Guid userId)
    {
        var (existChats, totalCount) = await chatService.SearchAsync(skip, take, userId);

        Response.Headers.Add("X-TotalCount", totalCount.ToString());

        return Ok(existChats);
    }

    /// <summary>
    /// Get chat by id
    /// </summary>
    /// <param name="chatId">Chat id</param>
    /// <response code="200">Chat</response>
    /// <response code="404">Chat not fount</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{chatId:guid}")]
    public async Task<ActionResult<Guid>> GetByIdAsync([FromRoute] Guid chatId)
    {
        var entity = await chatService.GetByIdAsync(chatId);

        if (entity is null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    /// <summary>
    /// Create new chat connection
    /// </summary>
    /// <param name="model">Create chat model</param>
    /// <response code="200">Connection successfully created</response>
    /// <response code="400"></response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<ActionResult<Chat>> CreateNewChatAsync([FromBody] CreateChatModel model)
    {
        var (result, id) = await chatService.CreateAsync(model.Name, model.Users);

        if (!result.Successed)
        {
            return BadRequest(result.Errors);
        }

        return Created(string.Empty, await chatService.GetByIdAsync(id));
    }

    /// <summary>
    /// Update chat
    /// </summary>
    /// <param name="chatId">Chat id</param>
    /// <param name="model">Update chat model</param>
    /// <response code="200">Chat successfully updated</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{chatId:guid}")]
    public async Task<ActionResult> UpdateAsync([FromRoute] Guid chatId, [FromBody] UpdateChatModel model)
    {
        var result = await chatService.UpdateAsync(chatId, model.Name, model.Users);

        if (!result.Successed)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    /// <summary>
    /// Delete chat
    /// </summary>
    /// <param name="chatId">Chat id</param>
    /// <response code="204">Chat successfully deleted</response>
    [HttpDelete("{chatId:guid}")]
    public async Task<ActionResult> DeleteChatAsync([FromRoute] Guid chatId)
    {
        await chatService.DeleteAsync(chatId);

        return NoContent();
    }
}

public record CreateChatModel(string Name, List<Guid> Users);

public record UpdateChatModel(string Name, List<Guid> Users);