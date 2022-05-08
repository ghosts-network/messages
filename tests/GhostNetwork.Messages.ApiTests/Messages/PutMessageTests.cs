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
public class PutMessageTests
{
    [Test]
    public async Task UpdateMessage_NoContent()
    {
        // Arrange
        var model = new UpdateMessageModel(Guid.NewGuid(), "message");
        var chatId = new Id(Guid.NewGuid().ToString());
        var now = DateTimeOffset.UtcNow;
        var message = new Message(new Id(Guid.NewGuid().ToString()), chatId, new UserInfo(model.SenderId, "Name", null), now, now, model.Message);

        var chatsServiceMock = new Mock<IChatsService>();
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
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => userMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chatId}/messages/{message.Id}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Test]
    public async Task UpdateMessage_Author_NotFound()
    {
        // Arrange
        var model = new UpdateMessageModel(Guid.NewGuid(), "Message");
        var id = new Id(Guid.NewGuid().ToString());
        var chatId = new Id(Guid.NewGuid().ToString());
        var now = DateTimeOffset.UtcNow;
        var message = new Message(id, chatId, new UserInfo(model.SenderId, It.IsAny<string>(), It.IsAny<string>()), now, now, model.Message);

        var chatsServiceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();
        var messageServiceMock = new Mock<IMessagesService>();

        userServiceMock
            .Setup(x => x.GetByIdAsync(model.SenderId))
            .ReturnsAsync(default(UserInfo));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => messageServiceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chatId}/messages/{message.Id}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task UpdateMessage_NullMessage_BadRequest()
    {
        // Arrange
        var model = new UpdateMessageModel(Guid.NewGuid(), null);
        var chatId = new Id(Guid.NewGuid().ToString());
        var now = DateTimeOffset.UtcNow;
        var message = new Message(new Id(Guid.NewGuid().ToString()), chatId, new UserInfo(model.SenderId, "Name", null), now, now, model.Message);

        var chatsServiceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();
        var messageServiceMock = new Mock<IMessagesService>();

        userServiceMock
            .Setup(x => x.GetByIdAsync(model.SenderId))
            .ReturnsAsync(new UserInfo(model.SenderId, "Name", null));

        messageServiceMock
            .Setup(x => x.UpdateAsync(message.Id, model.Message, model.SenderId))
            .ReturnsAsync(DomainResult.Error("Null message"));

        messageServiceMock
            .Setup(x => x.GetByIdAsync(message.Id))
            .ReturnsAsync(message);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => messageServiceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chatId}/messages/{message.Id}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}