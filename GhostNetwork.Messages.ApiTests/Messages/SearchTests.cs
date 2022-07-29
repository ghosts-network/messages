using System;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Messages.Chats;
using GhostNetwork.Messages.Domain;
using GhostNetwork.Messages.Users;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using Filter = GhostNetwork.Messages.Domain.Filter;

namespace GhostNetwork.Messages.ApiTests.Messages;

[TestFixture]
public class SearchTests
{
    [Test]
    public async Task Ok()
    {
        // Arrange
        var chatId = ObjectId.GenerateNewId().ToString();

        var now = DateTimeOffset.UtcNow;
        var author = new UserInfo(Guid.NewGuid(), "Name", null);
        var message = new Message(ObjectId.GenerateNewId().ToString(), chatId, author, now, now, "Test");

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(c => c.SearchAsync(It.IsAny<Filter>(), It.IsAny<Pagination>()))
            .ReturnsAsync((new[] { message }, long.MinValue));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.GetAsync($"/chats/{chatId}/messages");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Test]
    public async Task InvalidLimit()
    {
        // Arrange
        var chatId = ObjectId.GenerateNewId().ToString();

        var now = DateTimeOffset.UtcNow;
        var author = new UserInfo(Guid.NewGuid(), "Name", null);
        var message = new Message(ObjectId.GenerateNewId().ToString(), chatId, author, now, now, "Test");

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(c => c.SearchAsync(It.IsAny<Filter>(), It.IsAny<Pagination>()))
            .ReturnsAsync((new[] { message }, long.MinValue));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.GetAsync($"/chats/{chatId}/messages?limit=0");

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}