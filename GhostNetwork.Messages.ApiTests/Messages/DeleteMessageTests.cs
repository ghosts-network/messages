using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain;
using GhostNetwork.Messages.Chats;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
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
        var chatId = ObjectId.GenerateNewId();
        var messageId = ObjectId.GenerateNewId();

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(c => c.DeleteAsync(messageId))
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
    public async Task DeleteMessage_NotFound_1()
    {
        // Arrange
        var chatId = ObjectId.GenerateNewId();
        var messageId = ObjectId.GenerateNewId();

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(c => c.DeleteAsync(messageId))
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
    public async Task DeleteMessage_NotFound_2()
    {
        // Arrange
        var chatId = ObjectId.GenerateNewId();
        var messageId = ObjectId.GenerateNewId();

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(c => c.DeleteAsync(messageId))
            .ReturnsAsync(true);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.DeleteAsync($"/chats/{chatId}/messages/invalid_id");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}