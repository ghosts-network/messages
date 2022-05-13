using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Messages.Chats;
using GhostNetwork.Messages.Domain;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Messages.ApiTests.Messages;

[TestFixture]
public class DeleteTests
{
    [Test]
    public async Task Deleted()
    {
        // Arrange
        var chatId = ObjectId.GenerateNewId().ToString();
        var messageId = ObjectId.GenerateNewId().ToString();

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(c => c.DeleteAsync(chatId, messageId))
            .ReturnsAsync(true);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.DeleteAsync($"/chats/{chatId}/messages/{messageId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Test]
    public async Task NotFound_1()
    {
        // Arrange
        var chatId = ObjectId.GenerateNewId().ToString();
        var messageId = ObjectId.GenerateNewId().ToString();

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(c => c.DeleteAsync(chatId, messageId))
            .ReturnsAsync(false);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.DeleteAsync($"/chats/{chatId}/messages/{messageId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task NotFound_2()
    {
        // Arrange
        var chatId = "invalid_id";
        var messageId = ObjectId.GenerateNewId().ToString();

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(c => c.DeleteAsync(chatId, messageId))
            .ReturnsAsync(true);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.DeleteAsync($"/chats/{chatId}/messages/{messageId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task NotFound_3()
    {
        // Arrange
        var chatId = ObjectId.GenerateNewId().ToString();
        var messageId = "invalid_id";

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(c => c.DeleteAsync(chatId, messageId))
            .ReturnsAsync(true);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.DeleteAsync($"/chats/{chatId}/messages/{messageId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}