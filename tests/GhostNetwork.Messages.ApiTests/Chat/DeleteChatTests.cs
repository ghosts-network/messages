using System;
using System.Net;
using System.Threading.Tasks;
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

        var serviceMock = new Mock<IChatService>();
        serviceMock
            .Setup(x => x.DeleteChatAsync(chatId));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
        });
        
        //Act
        var response = await client.DeleteAsync($"/Chat/{chatId}");
        
        //Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }
}