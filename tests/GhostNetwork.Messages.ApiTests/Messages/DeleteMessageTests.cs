using System;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Messages.Messages;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Messages.ApiTests.Messages;

[TestFixture]
public class DeleteMessageTests
{
    [Test]
    public async Task DeleteMessage_NoContent()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var messageId = Guid.NewGuid().ToString();

        var message = new Message(messageId, chatId, It.IsAny<UserInfo>(), DateTimeOffset.Now, false, "some");

        var userMock = new Mock<IUserProvider>();
        var serviceMock = new Mock<IMessagesService>();

        serviceMock
            .Setup(x => x.GetByIdAsync(messageId))
            .ReturnsAsync(message);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => userMock.Object);
        });

        // Act
        var response = await client.DeleteAsync($"/chats/messages/{messageId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Test]
    public async Task DeleteMessage_NotFound()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var messageId = Guid.NewGuid().ToString();

        var userMock = new Mock<IUserProvider>();
        var serviceMock = new Mock<IMessagesService>();

        serviceMock
            .Setup(x => x.GetByIdAsync(messageId))
            .ReturnsAsync(default(Message));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => userMock.Object);
        });

        // Act
        var response = await client.DeleteAsync($"/chats/{chatId}/messages/{messageId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}