using System;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Messages.Chats;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Messages.ApiTests.Chat;

[TestFixture]
public class DeleteChatTests
{
    [Test]
    public async Task DeleteChat_NoContent()
    {
        //Setup
        var chatId = Guid.NewGuid();

        var chatServiceMock = new Mock<IChatService>();
        chatServiceMock
            .Setup(x => x.DeleteAsync(chatId));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatServiceMock.Object);
        });
        
        //Act
        var response = await client.DeleteAsync($"/chats/{chatId}");
        
        //Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }
}