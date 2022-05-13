using System;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Handlers.Messages;
using GhostNetwork.Messages.Chats;
using GhostNetwork.Messages.Domain;
using GhostNetwork.Messages.Users;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Messages.ApiTests.Messages;

[TestFixture]
public class UpdateTests
{
    [Test]
    public async Task Updated()
    {
        // Arrange
        var model = new UpdateMessageModel("test");
        var chat = new Chat(ObjectId.GenerateNewId().ToString(), "Test", new[]
        {
            new UserInfo(Guid.NewGuid(), "Test1", null),
            new UserInfo(Guid.NewGuid(), "Test2", null)
        });
        var now = DateTimeOffset.UtcNow;
        var author = new UserInfo(Guid.NewGuid(), "Name", null);
        var message = new Message(ObjectId.GenerateNewId().ToString(), chat.Id, author, now, now, "Test");

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(x => x.GetByIdAsync(chat.Id, message.Id))
            .ReturnsAsync(message);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chat.Id}/messages/{message.Id}", model.AsJsonContent());

        // Assert
        messagesStorageMock.Verify(x => x.UpdateAsync(It.IsAny<Message>()), Times.Once());
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Test]
    public async Task NotFound_1()
    {
        // Arrange
        var model = new UpdateMessageModel("test");
        var chat = new Chat(ObjectId.GenerateNewId().ToString(), "Test", new[]
        {
            new UserInfo(Guid.NewGuid(), "Test1", null),
            new UserInfo(Guid.NewGuid(), "Test2", null)
        });
        var now = DateTimeOffset.UtcNow;
        var author = new UserInfo(Guid.NewGuid(), "Name", null);
        var message = new Message(ObjectId.GenerateNewId().ToString(), chat.Id, author, now, now, "Test");

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(x => x.GetByIdAsync(chat.Id, message.Id))
            .ReturnsAsync(default(Message));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chat.Id}/messages/{message.Id}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task NotFound_2()
    {
        // Arrange
        var model = new UpdateMessageModel("test");
        var chat = new Chat("invalid_id", "Test", new[]
        {
            new UserInfo(Guid.NewGuid(), "Test1", null),
            new UserInfo(Guid.NewGuid(), "Test2", null)
        });
        var now = DateTimeOffset.UtcNow;
        var author = new UserInfo(Guid.NewGuid(), "Name", null);
        var message = new Message(ObjectId.GenerateNewId().ToString(), chat.Id, author, now, now, "Test");

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(x => x.GetByIdAsync(chat.Id, message.Id))
            .ReturnsAsync(message);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chat.Id}/messages/invalid_id", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task NotFound_3()
    {
        // Arrange
        var model = new UpdateMessageModel("test");
        var chat = new Chat(ObjectId.GenerateNewId().ToString(), "Test", new[]
        {
            new UserInfo(Guid.NewGuid(), "Test1", null),
            new UserInfo(Guid.NewGuid(), "Test2", null)
        });
        var now = DateTimeOffset.UtcNow;
        var author = new UserInfo(Guid.NewGuid(), "Name", null);
        var message = new Message("invalid_id", chat.Id, author, now, now, "Test");

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(x => x.GetByIdAsync(chat.Id, message.Id))
            .ReturnsAsync(message);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chat.Id}/messages/{message.Id}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus")]
    public async Task InvalidContent(string content)
    {
        // Arrange
        var model = new UpdateMessageModel(content);
        var chat = new Chat(ObjectId.GenerateNewId().ToString(), "Test", new[]
        {
            new UserInfo(Guid.NewGuid(), "Test1", null),
            new UserInfo(Guid.NewGuid(), "Test2", null)
        });
        var now = DateTimeOffset.UtcNow;
        var author = new UserInfo(Guid.NewGuid(), "Name", null);
        var message = new Message(ObjectId.GenerateNewId().ToString(), chat.Id, author, now, now, "Test");

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        messagesStorageMock
            .Setup(x => x.GetByIdAsync(chat.Id, message.Id))
            .ReturnsAsync(message);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chat.Id}/messages/{message.Id}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}