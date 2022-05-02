using System;
using System.Collections.Generic;
using System.Linq;
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
public class PostChatTests
{
    [Test]
    public async Task CreateNewChat_Created()
    {
        // Arrange
        var participantId = Guid.NewGuid();
        var model = new UpdateChatModel("Name", new List<string>() { participantId.ToString() });
        var participants = new List<UserInfo>() { new UserInfo(participantId, "UserName", null) };
        var chat = Chats.Chat.NewChat(model.Name, participants);

        var chatsServiceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();

        userServiceMock
            .Setup(x => x.SearchAsync(model.Participants))
            .ReturnsAsync(participants);

        chatsServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<string>(), participants))
            .ReturnsAsync((DomainResult.Success(), chat));

        chatsServiceMock
            .Setup(x => x.GetByIdAsync(chat.Id))
            .ReturnsAsync(chat);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.PostAsync("/chats/", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
    }

    [Test]
    public async Task Create_EmptyName_BadRequest()
    {
        // Arrange
        var model = new UpdateChatModel(null, new List<string>() { Guid.NewGuid().ToString() });

        var chatsServiceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();

        chatsServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<List<UserInfo>>()))
            .ReturnsAsync((DomainResult.Error("Some error"), default));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.PostAsync("/chats/", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Test]
    public async Task Create_NullParticipants_BadRequest()
    {
        // Arrange
        var model = new UpdateChatModel("Name", null);

        var chat = Chats.Chat.NewChat(model.Name, It.IsAny<List<UserInfo>>());

        var chatsServiceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();

        chatsServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<string>(), It.IsAny<List<UserInfo>>()))
            .ReturnsAsync((DomainResult.Success(), chat));

        chatsServiceMock
            .Setup(x => x.GetByIdAsync(chat.Id))
            .ReturnsAsync(chat);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.PostAsync("/chats/", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Test]
    public async Task Create_EmptyParticipants_BadRequest()
    {
        // Arrange
        var model = new UpdateChatModel("Name", new List<string>());
        var participants = Enumerable.Empty<UserInfo>().ToList();

        var chatsServiceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();

        userServiceMock
            .Setup(x => x.SearchAsync(model.Participants))
            .ReturnsAsync(participants);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.PostAsync("/chats/", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}