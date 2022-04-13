using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Messages.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Get exist user chats by user id
        /// </summary>
        /// <param name="skip">Skip exist chats up to a specified position</param>
        /// <param name="take">Take exist chats up to a specified position</param>
        /// <param name="userId">Filters by user</param>
        /// <response code="200">User chat ids</response>
        [HttpGet("search/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "X-TotalCount", "Number", "Total number of user chats")]
        public async Task<ActionResult> SearchExistChatsAsync(
            [FromQuery, Range(0, int.MaxValue)] int skip,
            [FromQuery, Range(1, 100)] int take,
            [FromRoute] Guid userId)
        {
            var (existChats, totalCount) = await _chatService.SearchChatsAsync(skip, take, userId);

            Response.Headers.Add("X-TotalCount", totalCount.ToString());
            Response.Headers.Add("X-HasMore", (totalCount > skip + take).ToString());

            return Ok(existChats);
        }

        /// <summary>
        /// Get chat info
        /// </summary>
        /// <param name="chatId">Chat id</param>
        [HttpGet("{chatId:guid}")]
        public async Task<ActionResult<Guid>> GetExistChatAsync([FromRoute] Guid chatId)
        {
            var entity = await _chatService.GetChatByIdAsync(chatId);

            return Ok(entity);
        }

        /// <summary>
        /// Create new chat connection
        /// </summary>
        /// <param name="users">List of user guids to add to the new chat</param>
        /// <response code="200">Connection successfully created</response>
        /// <response code="400"></response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<Guid>> CreateNewChatAsync([FromBody] IEnumerable<Guid> users)
        {
            var result = await _chatService.CreateNewChatAsync(users);

            return Ok(result);
        }

        /// <summary>
        /// Add new users to the chat
        /// </summary>
        /// <param name="chatId">Chat id</param>
        /// <param name="users">New users</param>
        /// <response code="200">Users successfully added</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{chatId:guid}")]
        public async Task<ActionResult> AddNewUsersToChatAsync([FromRoute] Guid chatId, [FromBody] IEnumerable<Guid> users)
        {
            await _chatService.AddNewUsersToChatAsync(chatId, users);

            return NoContent();
        }

        /// <summary>
        /// Delete exist chat
        /// </summary>
        /// <param name="chatId">Chat id</param>
        /// <response code="204">Chat successfully deleted</response>
        [HttpDelete("{chatId:guid}")]
        public async Task<ActionResult> DeleteChatAsync([FromRoute] Guid chatId)
        {
            await _chatService.DeleteChatAsync(chatId);

            return NoContent();
        }

        /// <summary>
        /// Get chat history by chat id
        /// </summary>
        /// <param name="chatId">chat id</param>
        /// <param name="skip">Skip exist messages up to a specified position</param>
        /// <param name="take">Take exist messages up to a specified position</param>
        /// <response code="200">Chat history</response>
        [HttpGet("{chatId:guid}/history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Message>>> GetChatHistoryAsync(
            [FromRoute] Guid chatId,
            [FromQuery, Range(0, int.MaxValue)] int skip,
            [FromQuery, Range(1, 100)] int take)
        {
            var (messages, totalCount) = await _chatService.GetChatHistoryAsync(skip, take, chatId);

            Response.Headers.Add("X-TotalCount", totalCount.ToString());
            Response.Headers.Add("X-HasMore", (totalCount > skip + take).ToString());

            return Ok(messages);
        }

        /// <summary>
        /// Send new message
        /// </summary>
        /// <param name="model">message model</param>
        /// <response code="200"></response>
        /// <response code="400"></response>
        [HttpPost("message")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SendMessageAsync([FromBody] CreateMessageModel model)
        {
            var (result, message) = await _chatService.SendMessageAsync(model.ChatId, model.SenderId, model.Message);

            if (!result.Successed)
            {
                return BadRequest(result.Errors);
            }

            return Ok(message);
        }

        /// <summary>
        /// Update message
        /// </summary>
        /// <param name="messageId">Message id</param>
        /// <param name="message">Updated message</param>
        /// <response code="204">Successfully updated</response>
        /// <response code="400">Smt went wrong</response>
        [HttpPut("message/{messageId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateMessageAsync(
            [FromRoute] Guid messageId,
            [FromBody] UpdateMessageModel model)
        {
            var result = await _chatService.UpdateMessageAsync(messageId, model.Message);

            if (!result.Successed)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        /// <summary>
        /// Delete message
        /// </summary>
        /// <param name="messageId">message id</param>
        /// <response code="204">Message successfully deleted</response>
        [HttpDelete("message/{messageId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteMessageAsync(
            [FromRoute] Guid messageId)
        {
            await _chatService.DeleteMessageAsync(messageId);

            return NoContent();
        }
    }

    public record CreateMessageModel(Guid ChatId, Guid SenderId, string Message);

    public record UpdateMessageModel(string Message);
}