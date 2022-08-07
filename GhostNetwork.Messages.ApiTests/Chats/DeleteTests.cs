using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain.Chats;
using GhostNetwork.Messages.Api.Domain.Messages;
using GhostNetwork.Messages.Api.Domain.Users;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Messages.ApiTests.Chats;

[TestFixture]
public class DeleteTests
{
    [Test]
    public async Task Deleted()
    {
        // Arrange
        var chatId = ObjectId.GenerateNewId().ToString();

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        chatsStorageMock
            .Setup(c => c.DeleteAsync(chatId))
            .ReturnsAsync(true);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.DeleteAsync($"/chats/{chatId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Test]
    public async Task NotFount_1()
    {
        // Arrange
        var chatId = ObjectId.GenerateNewId().ToString();

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        chatsStorageMock
            .Setup(c => c.DeleteAsync(chatId))
            .ReturnsAsync(false);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.DeleteAsync($"/chats/{chatId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task NotFount_2()
    {
        // Arrange
        var chatId = "invalid_id";

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        chatsStorageMock
            .Setup(c => c.DeleteAsync(chatId))
            .ReturnsAsync(true);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.DeleteAsync($"/chats/{chatId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}