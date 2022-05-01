using System;
using System.Net;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Messages.Api.Controllers;
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
        var model = new CreateMessageModel("id", "message");
        var chatId = Guid.NewGuid();
        var message = new Message("Guid.Empty", chatId, null, DateTimeOffset.Now, false, model.Message);

        var userMock = new Mock<IUserProvider>();
        var serviceMock = new Mock<IMessagesService>();

        serviceMock
            .Setup(x => x.SendAsync(chatId, It.IsAny<UserInfo>(), model.Message))
            .ReturnsAsync((DomainResult.Success(), message.Id));

        serviceMock
            .Setup(x => x.GetByIdAsync(message.Id))
            .ReturnsAsync(message);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => userMock.Object);
        });

        // Act
        var response = await client.PostAsync($"/chats/{chatId}/messages", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
    }

    [Test]
    public async Task SendMessage_BadRequest()
    {
        // Arrange
        var model = new CreateMessageModel("id", "message");
        var chatId = Guid.NewGuid();

        var userMock = new Mock<IUserProvider>();
        var serviceMock = new Mock<IMessagesService>();

        serviceMock
            .Setup(x => x.SendAsync(chatId, It.IsAny<UserInfo>(), model.Message))
            .ReturnsAsync((DomainResult.Error(string.Empty), default));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => userMock.Object);
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
        var model = new CreateMessageModel("id", "message");
        var invalidChatId = Guid.NewGuid();

        var userMock = new Mock<IUserProvider>();
        var serviceMock = new Mock<IMessagesService>();

        serviceMock
            .Setup(x => x.SendAsync(invalidChatId, It.IsAny<UserInfo>(), model.Message))
            .ReturnsAsync((DomainResult.Success(), default));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => userMock.Object);
        });

        // Act
        var response = await client.PostAsync($"/chats/{invalidChatId}/messages", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}