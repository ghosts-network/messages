using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Messages.ApiTests.Messages;

[TestFixture]
public class GetMessageTests
{
    [Test]
    public async Task GetChatMessages()
    {
        //Setup
        var chatId = Guid.NewGuid();
        const int skip = 0;
        const int take = 1;

        var message = new Message(Guid.NewGuid(), chatId, Guid.NewGuid(), DateTimeOffset.Now, false, "Test");
        var messages = new List<Message>
        {
            message
        };

        var serviceMock = new Mock<IMessageService>();

        serviceMock
            .Setup(x => x.SearchAsync(skip, take, chatId))
            .ReturnsAsync((messages, messages.Count));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
        });
        
        //Act
        var response = await client.GetAsync($"/chats/{chatId}/messages");
        
        //Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}