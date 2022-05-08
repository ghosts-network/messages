using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Messages.Chats;
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
        var chatId = new Id(Guid.NewGuid().ToString());
        var id = new Id(Guid.NewGuid().ToString());

        var now = DateTimeOffset.UtcNow;
        var message = new Message(id, chatId, new UserInfo(Guid.NewGuid(), "Name", null), now, now, "Test");

        var messages = new List<Message> { message };

        var chatsServiceMock = new Mock<IChatsService>();
        var userMock = new Mock<IUserProvider>();
        var messagesServiceMock = new Mock<IMessagesService>();

        messagesServiceMock
            .Setup(x => x.SearchAsync(It.IsAny<MessageFilter>(), It.IsAny<Pagination>()))
            .ReturnsAsync(messages);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => messagesServiceMock.Object);
            collection.AddScoped(_ => userMock.Object);
        });

        // Act
        var response = await client.GetAsync($"/chats/{chatId}/messages");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Test]
    public async Task GetById_NotFound()
    {
        // Arrange
        var id = new Id(Guid.NewGuid().ToString());

        var chatsServiceMock = new Mock<IChatsService>();
        var userMock = new Mock<IUserProvider>();
        var serviceMock = new Mock<IMessagesService>();

        serviceMock
            .Setup(x => x.GetByIdAsync(id))
            .ReturnsAsync(default(Message));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => userMock.Object);
        });

        // Act
        var response = await client.GetAsync($"/chats/messages/{id}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task GetById_Ok()
    {
        // Arrange
        var chatId = new Id(Guid.NewGuid().ToString());
        var messageId = new Id(Guid.NewGuid().ToString());

        var now = DateTimeOffset.UtcNow;
        var message = new Message(messageId, chatId, It.IsAny<UserInfo>(), DateTimeOffset.Now, now, "some");

        var chatsServiceMock = new Mock<IChatsService>();
        var userMock = new Mock<IUserProvider>();
        var serviceMock = new Mock<IMessagesService>();

        serviceMock
            .Setup(x => x.GetByIdAsync(messageId))
            .ReturnsAsync(message);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => userMock.Object);
        });

        // Act
        var response = await client.GetAsync($"/chats/messages/{messageId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}