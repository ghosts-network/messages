using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Messages.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        /// <summary>
        /// Create comment
        /// </summary>
        /// <param name="chatId">Chat Id</param>
        /// <returns>Created comment</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> GetChatAsync([FromBody] int chatId)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult<string>> CreateChatAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult<bool>> DeleteChat([FromBody] int chatId)
        {
            throw new NotImplementedException();
        }
    }
}