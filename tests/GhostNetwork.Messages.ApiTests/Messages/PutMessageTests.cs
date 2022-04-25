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
public class PutMessageTests
{
    [Test]
    public async Task UpdateMessage_NoContent()
    {
        // Arrange
        var model = new UpdateMessageModel(Guid.NewGuid(), "message");
        var chatId = Guid.NewGuid();
        var message = new Message(Guid.NewGuid().ToString(), chatId, new UserInfo(model.SenderId, "Name", null), DateTimeOffset.Now, false, model.Message);

        var userMock = new Mock<IUserProvider>();
        var serviceMock = new Mock<IMessagesService>();

        serviceMock
            .Setup(x => x.UpdateAsync(message.Id, model.Message, model.SenderId))
            .ReturnsAsync(DomainResult.Success);

        serviceMock
            .Setup(x => x.GetByIdAsync(message.Id))
            .ReturnsAsync(message);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => userMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/messages/{message.Id}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Test]
    public async Task UpdateMessage_NotFound()
    {
        // Arrange
        var model = new UpdateMessageModel(Guid.NewGuid(), It.IsAny<string>());
        var chatId = Guid.NewGuid();
        var message = new Message("some_id", chatId, new UserInfo(model.SenderId, It.IsAny<string>(), It.IsAny<string>()), DateTimeOffset.Now, false, model.Message);

        var userMock = new Mock<IUserProvider>();
        var serviceMock = new Mock<IMessagesService>();

        serviceMock
            .Setup(x => x.UpdateAsync(message.Id, model.Message, model.SenderId))
            .ReturnsAsync(DomainResult.Error(string.Empty));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => userMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/messages/{message.Id}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task UpdateMessage_BadRequest()
    {
        // Arrange
        var model = new UpdateMessageModel(Guid.NewGuid(), It.IsAny<string>());
        var chatId = Guid.NewGuid();
        var moqAuthor = Guid.NewGuid();
        var message = new Message("some_id", chatId, new UserInfo(model.SenderId, It.IsAny<string>(), It.IsAny<string>()), DateTimeOffset.Now, false, model.Message);

        var userMock = new Mock<IUserProvider>();
        var serviceMock = new Mock<IMessagesService>();

        serviceMock
            .Setup(x => x.UpdateAsync(message.Id, model.Message, moqAuthor))
            .ReturnsAsync(DomainResult.Success);

        serviceMock
            .Setup(x => x.GetByIdAsync(message.Id))
            .ReturnsAsync(default(Message));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => userMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/messages/{message.Id}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}