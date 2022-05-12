using System;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain;
using GhostNetwork.Messages.Chats;
using GhostNetwork.Messages.Users;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Messages.ApiTests.Messages;

[TestFixture]
public class GetMessageTests
{
    [Test]
    public async Task GetById_Ok()
    {
        // Arrange
        var chatId = ObjectId.GenerateNewId().ToString();

        var now = DateTimeOffset.UtcNow;
        var author = new UserInfo(Guid.NewGuid(), "Name", null);
        var message = new Message(ObjectId.GenerateNewId().ToString(), chatId, author, now, now, "Test");

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(c => c.GetByIdAsync(chatId, message.Id))
            .ReturnsAsync(message);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.GetAsync($"/chats/{chatId}/messages/{message.Id}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Test]
    public async Task GetById_NotFound_1()
    {
        // Arrange
        var chatId = ObjectId.GenerateNewId().ToString();
        var messageId = ObjectId.GenerateNewId().ToString();

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(c => c.GetByIdAsync(chatId, messageId))
            .ReturnsAsync(default(Message));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.GetAsync($"/chats/{chatId}/messages/{messageId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task GetById_NotFound_2()
    {
        // Arrange
        var chatId = ObjectId.GenerateNewId().ToString();
        var messageId = ObjectId.GenerateNewId().ToString();

        var now = DateTimeOffset.UtcNow;
        var author = new UserInfo(Guid.NewGuid(), "Name", null);
        var message = new Message(messageId, chatId, author, now, now, "Test");

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(c => c.GetByIdAsync(chatId, messageId))
            .ReturnsAsync(message);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.GetAsync($"/chats/{chatId}/messages/invalid_id");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}