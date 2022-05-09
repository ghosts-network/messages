﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain;
using GhostNetwork.Messages.Chats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Filter = GhostNetwork.Messages.Api.Domain.Filter;

namespace GhostNetwork.Messages.Api.Controllers;

[ApiController]
[Route("chats")]
public class MessagesController : ControllerBase
{
    private readonly IMessagesStorage messagesStorage;
    private readonly IChatsStorage chatsStorage;

    public MessagesController(IMessagesStorage messagesStorage, IChatsStorage chatsStorage)
    {
        this.messagesStorage = messagesStorage;
        this.chatsStorage = chatsStorage;
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
        if (!ObjectId.TryParse(chatId, out var chatObjectId))
        {
            return NotFound();
        }

        var filter = new Filter(chatObjectId);
        var paging = new Pagination(cursor, limit);

        var messages = await messagesStorage.SearchAsync(filter, paging);

        return Ok(messages);
    }

    /// <summary>
    /// Get message by id
    /// </summary>
    /// <param name="messageId">Message id</param>
    /// <response code="200">Message</response>
    /// <response code="404">Message is not found</response>
    [HttpGet("{chatId}/messages/{messageId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Message>> GetByIdAsync([FromRoute] string messageId)
    {
        if (!ObjectId.TryParse(messageId, out var objectId))
        {
            return NotFound();
        }

        var message = await messagesStorage.GetByIdAsync(objectId);

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
        if (!ObjectId.TryParse(chatId, out var chatObjectId))
        {
            return NotFound();
        }

        var chat = await chatsStorage.GetByIdAsync(chatObjectId);
        if (chat == null)
        {
            return NotFound();
        }

        var author = chat.Participants.FirstOrDefault(p => p.Id == model.SenderId);
        if (author == null)
        {
            return BadRequest(new ProblemDetails { Title = "Sender is not in chat" });
        }

        var now = DateTimeOffset.UtcNow;
        var message = new Message(ObjectId.GenerateNewId(), chat.Id, author, now, now, model.Content);
        await messagesStorage.InsertAsync(message);

        await chatsStorage.ReorderAsync(chat.Id);
        return Created(Url.Action("GetById", new { id = message.Id.ToString() }) ?? string.Empty, await messagesStorage.GetByIdAsync(message.Id));
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
        if (!ObjectId.TryParse(messageId, out var messageObjectId))
        {
            return NotFound();
        }

        var message = await messagesStorage.GetByIdAsync(messageObjectId);

        if (message is null)
        {
            return NotFound();
        }

        message = message with { Content = model.Content };
        await messagesStorage.UpdateAsync(message);

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
    [HttpDelete("{chatId}/messages/{messageId}")]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] string messageId)
    {
        if (!ObjectId.TryParse(messageId, out var messageObjectId))
        {
            return NotFound();
        }

        if (await messagesStorage.DeleteAsync(messageObjectId))
        {
            return NoContent();
        }

        return NotFound();
    }
}

public record CreateMessageModel(
    [Required] Guid SenderId,
    [Required, StringLength(500)] string Content);

public record UpdateMessageModel(
    [Required, StringLength(500)] string Content);
