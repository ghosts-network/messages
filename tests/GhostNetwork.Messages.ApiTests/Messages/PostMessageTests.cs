using System;
using System.Net;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Messages.Api.Controllers;
using GhostNetwork.Messages.Chats;
using GhostNetwork.Messages.Messages;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Messages.ApiTests.Messages;

[TestFixture]
public class PostMessageTests
{
    [Test]
    public async Task SendMessage_Ok()
    {
        // Arrange
        var model = new CreateMessageModel(Guid.NewGuid(), "message");
        var chatId = new Id(Guid.NewGuid().ToString());
        var now = DateTimeOffset.UtcNow;
        var message = new Message(new Id(Guid.NewGuid().ToString()), chatId, null, now, now, model.Message);

        var chatsServiceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();
        var messagesServiceMock = new Mock<IMessagesService>();

        chatsServiceMock
            .Setup(x => x.GetByIdAsync(chatId))
            .ReturnsAsync(new Chats.Chat(chatId, "Name", new[]
            {
                new UserInfo(Guid.NewGuid(), "Name", null)
            }));

        userServiceMock
            .Setup(x => x.GetByIdAsync(model.SenderId))
            .ReturnsAsync(new UserInfo(Guid.NewGuid(), "Name", null));

        messagesServiceMock
            .Setup(x => x.SendAsync(chatId, It.IsAny<UserInfo>(), model.Message))
            .ReturnsAsync((DomainResult.Success(), message.Id));

        messagesServiceMock
            .Setup(x => x.GetByIdAsync(message.Id))
            .ReturnsAsync(message);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => messagesServiceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.PostAsync($"/chats/{chatId}/messages", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
    }

    [Test]
    public async Task SendMessage_NullAuthor_BadRequest()
    {
        // Arrange
        var model = new CreateMessageModel(Guid.Empty, "message");
        var chatId = new Id(Guid.NewGuid().ToString());

        var chatsServiceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();
        var messagesServiceMock = new Mock<IMessagesService>();

        chatsServiceMock
            .Setup(x => x.GetByIdAsync(chatId))
            .ReturnsAsync(new Chats.Chat(chatId, "Name", new[]
            {
                new UserInfo(Guid.NewGuid(), "Name", null)
            }));

        userServiceMock
            .Setup(x => x.GetByIdAsync(model.SenderId))
            .ReturnsAsync(default(UserInfo));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => messagesServiceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.PostAsync($"/chats/{chatId}/messages", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        var responseModel = response.Content.AsProblemDetails();
        Assert.AreEqual("Author is not found", responseModel.Title);
    }

    [Test]
    public async Task SendMessage_NullMessage_BadRequest()
    {
        // Arrange
        var model = new CreateMessageModel(Guid.NewGuid(), null);
        var chatId = new Id(Guid.NewGuid().ToString());

        var chatsServiceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();
        var messagesServiceMock = new Mock<IMessagesService>();

        userServiceMock
            .Setup(x => x.GetByIdAsync(model.SenderId))
            .ReturnsAsync(new UserInfo(Guid.NewGuid(), "Name", null));

        messagesServiceMock
            .Setup(x => x.SendAsync(chatId, It.IsAny<UserInfo>(), model.Message))
            .ReturnsAsync((DomainResult.Error("Null message"), default));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => messagesServiceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.PostAsync($"/chats/{chatId}/messages", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Test]
    public async Task SendMessage_ChatNotFound_BadRequest()
    {
        // Arrange
        var model = new CreateMessageModel(Guid.NewGuid(), "message");
        var invalidChatId = new Id(Guid.NewGuid().ToString());

        var chatsServiceMock = new Mock<IChatsService>();
        var userMock = new Mock<IUserProvider>();
        var serviceMock = new Mock<IMessagesService>();

        serviceMock
            .Setup(x => x.SendAsync(invalidChatId, It.IsAny<UserInfo>(), model.Message))
            .ReturnsAsync((DomainResult.Success(), default));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => userMock.Object);
        });

        // Act
        var response = await client.PostAsync($"/chats/{invalidChatId}/messages", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}