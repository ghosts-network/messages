using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Messages.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<ChatHub, IChatHub> _hubContext;

        public ChatController(IChatService chatService, IHubContext<ChatHub, IChatHub> hubContext)
        {
            _chatService = chatService;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Get exist user chats
        /// </summary>
        /// <param name="skip">Skip exist chats up to a specified position</param>
        /// <param name="take">Take exist chats up to a specified position</param>
        /// <param name="userId">Filters by user</param>
        /// <response code="200">User chat ids</response>
        [HttpGet("connection/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "X-TotalCount", "Number", "Total number of user chats")]
        public async Task<ActionResult> SearchExistChatsAsync(
            [FromQuery, Range(0, int.MaxValue)] int skip,
            [FromQuery, Range(1, 100)] int take,
            [FromRoute] Guid userId)
        {
            var (existChats, totalCount) = await _chatService.SearchExistChatsAsync(skip, take, userId);

            Response.Headers.Add("X-TotalCount", totalCount.ToString());
            Response.Headers.Add("X-HasMore", (totalCount > skip + take).ToString());

            return Ok(existChats);
        }

        /// <summary>
        /// Probably test
        /// </summary>
        /// <param name="chatId">chat id</param>
        [HttpGet("connection/{chatId:guid}/test")]
        public async Task<ActionResult<Guid>> GetExistChatAsync([FromRoute] Guid chatId)
        {
            var entity = await _chatService.GetExistChatByIdAsync(chatId);

            return Ok(entity);
        }

        /// <summary>
        /// Create new chat connection
        /// </summary>
        /// <param name="model">Connection model</param>
        /// <response code="200">Connection successfully created</response>
        /// <response code="400"></response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("connection")]
        public async Task<ActionResult<Guid>> CreateNewChatAsync([FromBody] CreateChatModel model)
        {
            return default;
        }

        /// <summary>
        /// Delete exist chat
        /// </summary>
        /// <param name="chatId">chat id</param>
        /// <param name="userId">requester id</param>
        /// <response code="204">Chat successfully deleted</response>
        [HttpDelete("connection/{chatId:guid}")]
        public async Task<ActionResult> DeleteChatAsync([FromRoute] Guid chatId, [FromQuery] Guid userId)
        {
            return NoContent();
        }

        /// <summary>
        /// Get chat history by chat id
        /// </summary>
        /// <param name="chatId">chat id</param>
        /// <param name="userId">requester id</param>
        /// <response code="200"></response>
        /// <response code="400"></response>
        [HttpGet("history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Message>>> GetChatHistoryAsync(
            [FromRoute] Guid chatId,
            [FromRoute] Guid userId)
        {
            return Ok();
        }

        /// <summary>
        /// Send new message
        /// </summary>
        /// <param name="chatId">chat id</param>
        /// <param name="model">message model</param>
        /// <response code="200"></response>
        /// <response code="400"></response>
        [HttpPost("message")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SendMessageAsync([FromRoute] Guid chatId, [FromBody] CreateMessageModel model)
        {
            return Ok();
        }

        /// <summary>
        /// Update message
        /// </summary>
        /// <param name="chatId">chat id</param>
        /// <param name="messageId">message id</param>
        /// <param name="model">update message model</param>
        /// <response code="204"></response>
        /// <response code="400"></response>
        [HttpPut("message")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateMessageAsync(
            [FromRoute] Guid chatId,
            [FromRoute] Guid messageId,
            [FromBody] UpdateMessageModel model)
        {
            return NoContent();
        }

        /// <summary>
        /// Delete message
        /// </summary>
        /// <param name="chatId">chat id</param>
        /// <param name="messageId">message id</param>
        /// <param name="userId">requester id</param>
        /// <response code="204"></response>
        /// <response code="400"></response>
        [HttpDelete("message")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteMessageAsync(
            [FromRoute] Guid chatId,
            [FromRoute] Guid messageId,
            [FromQuery] Guid userId)
        {
            return NoContent();
        }
    }

    public record CreateChatModel(Guid SenderId, Guid RequesterId);

    public record CreateMessageModel(Guid SenderId, Guid ReceiverId, string Data);

    public record UpdateMessageModel(Guid SenderId, string Data);
}