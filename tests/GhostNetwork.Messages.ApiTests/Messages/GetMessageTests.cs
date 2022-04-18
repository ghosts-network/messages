using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Messages.Messages;
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
        // Arrange
        var chatId = Guid.NewGuid();
        var id = Guid.NewGuid().ToString();
        const int take = 1;

        var message = new Message(id, chatId, Guid.NewGuid(), DateTimeOffset.Now, false, "Test");

        var messages = new List<Message>()
        {
            message
        };

        var serviceMock = new Mock<IMessagesService>();

        serviceMock
            .Setup(x => x.SearchAsync(id, take, chatId))
            .ReturnsAsync((messages, messages.Count, messages[^1].Id));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
        });

        // Act
        var response = await client.GetAsync($"/chats/{chatId}/messages");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}