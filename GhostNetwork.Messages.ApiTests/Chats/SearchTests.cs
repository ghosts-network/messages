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
using Filter = GhostNetwork.Messages.Chats.Filter;

namespace GhostNetwork.Messages.ApiTests.Chats;

[TestFixture]
public class SearchTests
{
    [Test]
    public async Task Ok()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var chat = new Chat(ObjectId.GenerateNewId().ToString(), "Test", new[]
        {
            new UserInfo(Guid.NewGuid(), "Test1", null),
            new UserInfo(Guid.NewGuid(), "Test2", null)
        });

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        chatsStorageMock
            .Setup(x => x.SearchAsync(It.IsAny<Filter>(), It.IsAny<Pagination>()))
            .ReturnsAsync(new[] { chat });

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.GetAsync($"/chats?userId={userId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Test]
    public async Task InvalidLimit()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var chat = new Chat(ObjectId.GenerateNewId().ToString(), "Test", new[]
        {
            new UserInfo(Guid.NewGuid(), "Test1", null),
            new UserInfo(Guid.NewGuid(), "Test2", null)
        });

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        chatsStorageMock
            .Setup(x => x.SearchAsync(It.IsAny<Filter>(), It.IsAny<Pagination>()))
            .ReturnsAsync(new[] { chat });

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.GetAsync($"/chats?userId={userId}&limit=0");

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Test]
    public async Task InvalidUserId()
    {
        // Arrange
        var chat = new Chat(ObjectId.GenerateNewId().ToString(), "Test", new[]
        {
            new UserInfo(Guid.NewGuid(), "Test1", null),
            new UserInfo(Guid.NewGuid(), "Test2", null)
        });

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        chatsStorageMock
            .Setup(x => x.SearchAsync(It.IsAny<Filter>(), It.IsAny<Pagination>()))
            .ReturnsAsync(new[] { chat });

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.GetAsync("/chats");

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}