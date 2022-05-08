using System;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Messages.Chats;
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
        var chatId = new Id(Guid.NewGuid().ToString());
        var id = new Id(Guid.NewGuid().ToString());

        var now = DateTimeOffset.UtcNow;
        var message = new Message(id, chatId, It.IsAny<UserInfo>(), now, now, "some");

        var chatsServiceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();
        var messagesServiceMock = new Mock<IMessagesService>();

        messagesServiceMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(message);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => messagesServiceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.DeleteAsync($"/chats/messages/{id}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Test]
    public async Task DeleteMessage_NotFound()
    {
        // Arrange
        var id = new Id(Guid.NewGuid().ToString());

        var chatsServiceMock = new Mock<IChatsService>();
        var userMock = new Mock<IUserProvider>();
        var messagesServiceMock = new Mock<IMessagesService>();

        messagesServiceMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(default(Message));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => messagesServiceMock.Object);
            collection.AddScoped(_ => userMock.Object);
        });

        // Act
        var response = await client.DeleteAsync($"/chats/messages/{id}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}