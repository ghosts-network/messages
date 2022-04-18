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
        var chatId = Guid.NewGuid();
        var model = new CreateMessageModel(Guid.NewGuid(), "Test");

        var message = new Message(Guid.NewGuid().ToString(), chatId, model.SenderId, DateTimeOffset.Now, false, model.Message);

        var serviceMock = new Mock<IMessagesService>();

        serviceMock
            .Setup(x => x.SendAsync(chatId, model.SenderId, model.Message))
            .ReturnsAsync((DomainResult.Success(), message));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
        });

        // Act
        var response = await client.PostAsync($"/chats/{chatId}/messages", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Test]
    public async Task SendMessage_BadRequest()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var model = new CreateMessageModel(Guid.NewGuid(), "Test");

        var serviceMock = new Mock<IMessagesService>();

        serviceMock
            .Setup(x => x.SendAsync(chatId, model.SenderId, model.Message))
            .ReturnsAsync((DomainResult.Error(string.Empty), default));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
        });

        // Act
        var response = await client.PostAsync($"/chats/{chatId}/messages", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}