using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Messages.Api.Controllers;
using GhostNetwork.Messages.Chats;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Messages.ApiTests.Chat;

[TestFixture]
public class PutChatTests
{
    [Test]
    public async Task Update_NoContent()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var model = new UpdateChatModel("Name", new List<string> { participantId.ToString() });
        var participants = new List<UserInfo>() { new(participantId, "UserId", null) };
        var chat = new Chats.Chat(chatId, model.Name, It.IsAny<List<UserInfo>>());

        var chatsServiceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();

        userServiceMock
            .Setup(x => x.SearchAsync(model.Participants))
            .ReturnsAsync(participants);

        chatsServiceMock
            .Setup(x => x.UpdateAsync(chatId, model.Name, participants))
            .ReturnsAsync(DomainResult.Success());

        chatsServiceMock
            .Setup(x => x.GetByIdAsync(chatId))
            .ReturnsAsync(chat);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chatId}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Test]
    public async Task Update_EmptyName_BadRequest()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var model = new UpdateChatModel(null, new List<string> { participantId.ToString() });
        var participants = new List<UserInfo>() { new(participantId, "UserId", null) };

        var chatsServiceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();

        userServiceMock
            .Setup(x => x.SearchAsync(model.Participants))
            .ReturnsAsync(participants);

        chatsServiceMock
            .Setup(x => x.UpdateAsync(chatId, model.Name, participants))
            .ReturnsAsync(DomainResult.Error("Err"));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chatId}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Test]
    public async Task Update_EmptyParticipants_NotFound()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var model = new UpdateChatModel("Name", new List<string>());
        var participants = new List<UserInfo>();

        var chatsServiceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();

        userServiceMock
            .Setup(x => x.SearchAsync(model.Participants))
            .ReturnsAsync(participants);

        chatsServiceMock
            .Setup(x => x.UpdateAsync(chatId, model.Name, participants))
            .ReturnsAsync(DomainResult.Error("Err"));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chatId}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task Update_NullParticipants_BadRequest()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var model = new UpdateChatModel("Name", null);
        var participants = new List<UserInfo>() { new(participantId, "UserId", null) };

        var chatsServiceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();

        userServiceMock
            .Setup(x => x.SearchAsync(model.Participants))
            .ReturnsAsync(participants);

        chatsServiceMock
            .Setup(x => x.UpdateAsync(chatId, model.Name, participants))
            .ReturnsAsync(DomainResult.Error("Err"));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chatId}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}