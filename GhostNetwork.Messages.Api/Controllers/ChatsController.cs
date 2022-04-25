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
    /// Get exist user chats by user id
    /// </summary>
    /// <param name="skip">Skip exist chats up to a specified position</param>
    /// <param name="take">Take exist chats up to a specified position</param>
    /// <param name="userId">Filters by user</param>
    /// <response code="200">Exist user chats</response>
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
    /// <response code="201">Connection successfully created</response>
    /// <response code="400">Problem details</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<ActionResult<Chat>> CreateNewChatAsync([FromBody] CreateChatModel model)
    {
        var participants = await userProvider.SearchAsync(model.Participants);

        var (result, chat) = await chatService.CreateAsync(model.Name, participants.ToList());

        if (result.Successed)
        {
            return Created(Url.Action("GetById", new { chat.Id }) ?? string.Empty, chat);
        }

        return BadRequest(result.ToProblemDetails());
    }

    /// <summary>
    /// Update chat
    /// </summary>
    /// <param name="chatId">Chat id</param>
    /// <param name="model">Update chat model</param>
    /// <response code="204">Chat successfully updated</response>
    /// <response code="400">Problem details</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{chatId:guid}")]
    public async Task<ActionResult> UpdateAsync([FromRoute] Guid chatId, [FromBody] UpdateChatModel model)
    {
        var participants = await userProvider.SearchAsync(model.Participants);

        var result = await chatService.UpdateAsync(chatId, model.Name, participants.ToList());

        if (!result.Successed)
        {
            return BadRequest(result.ToProblemDetails());
        }

        return NoContent();
    }

    /// <summary>
    /// Delete chat
    /// </summary>
    /// <param name="chatId">Chat id</param>
    /// <response code="204">Chat successfully deleted</response>
    /// <response code="404">Chat is not found</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{chatId:guid}")]
    public async Task<ActionResult> DeleteChatAsync([FromRoute] Guid chatId)
    {
        var chat = await chatService.GetByIdAsync(chatId);

        if (chat is null)
        {
            return NotFound();
        }

        await chatService.DeleteAsync(chatId);

        return NoContent();
    }
}

public record CreateChatModel(string Name, List<string> Participants);

public record UpdateChatModel(string Name, List<string> Participants);