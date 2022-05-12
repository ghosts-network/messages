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
public class GetChatTests
{
    [Test]
    public async Task GetById_Ok()
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
            .Setup(x => x.GetByIdAsync(chat.Id))
            .ReturnsAsync(chat);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.GetAsync($"/chats/{chat.Id}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Test]
    public async Task GetById_NotFound_1()
    {
        // Arrange
        var chatId = ObjectId.GenerateNewId().ToString();

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        chatsStorageMock
            .Setup(x => x.GetByIdAsync(chatId))
            .ReturnsAsync(default(Chat));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.GetAsync($"/chats/{chatId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task GetById_NotFound_2()
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
            .Setup(x => x.GetByIdAsync(chat.Id))
            .ReturnsAsync(chat);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.GetAsync("/chats/invalid-id");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task Search_Ok()
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
}