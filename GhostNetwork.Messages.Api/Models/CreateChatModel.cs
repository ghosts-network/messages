using System.Collections.Generic;

namespace GhostNetwork.Messages.Api.Models
{
    public class CreateChatModel
    {
        public string ChatName { get; set; }
        
        public List<int> UserList { get; set; }
    }
}