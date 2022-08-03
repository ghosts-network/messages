using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Domain.Chats;
using GhostNetwork.Messages.Api.Domain.Messages;
using GhostNetwork.Messages.Api.Domain.Users;
using GhostNetwork.Messages.Api.Handlers.Chats;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Messages.ApiTests.Chats;

[TestFixture]
public class CreateTests
{
    [Test]
    public async Task Created()
    {
        // Arrange
        var p1 = new UserInfo(Guid.NewGuid(), "Test1", null);
        var p2 = new UserInfo(Guid.NewGuid(), "Test2", null);
        var model = new CreateChatModel("Chat name", new List<Guid> { p1.Id, p2.Id });

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        userStorageMock
            .Setup(x => x.SearchAsync(new List<Guid> { p1.Id, p2.Id }))
            .ReturnsAsync(new List<UserInfo> { p1, p2 });

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.PostAsync("/chats/", model.AsJsonContent());

        // Assert
        chatsStorageMock
            .Verify(x => x.InsertAsync(It.IsAny<Chat>()), Times.Once());

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("0123456789012345678900123456789001234567890012345678901", Description = "Too long name")]
    public async Task InvalidName(string name)
    {
        // Arrange
        var p1 = new UserInfo(Guid.NewGuid(), "Test1", null);
        var p2 = new UserInfo(Guid.NewGuid(), "Test2", null);
        var model = new CreateChatModel(name, new List<Guid> { p1.Id, p2.Id });

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        userStorageMock
            .Setup(x => x.SearchAsync(new List<Guid> { p1.Id, p2.Id }))
            .ReturnsAsync(new List<UserInfo> { p1, p2 });

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.PostAsync("/chats/", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestCaseSource(typeof(ParticipantCases))]
    public async Task InvalidParticipants(List<Guid> participants)
    {
        // Arrange
        var model = new CreateChatModel("Chat name", participants);

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        userStorageMock
            .Setup(x => x.SearchAsync(participants))
            .ReturnsAsync(new List<UserInfo>());

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.PostAsync("/chats/", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Test]
    public async Task SomeParticipantsNotExists()
    {
        // Arrange
        var p1 = new UserInfo(Guid.NewGuid(), "Test1", null);
        var p2 = new UserInfo(Guid.NewGuid(), "Test2", null);
        var model = new CreateChatModel("Chat name", new List<Guid> { p1.Id, p2.Id });

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        userStorageMock
            .Setup(x => x.SearchAsync(new List<Guid> { p1.Id, p2.Id }))
            .ReturnsAsync(new List<UserInfo> { p1 });

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.PostAsync("/chats/", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private class ParticipantCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return null;
            yield return Enumerable.Empty<Guid>().ToList();
            yield return Enumerable.Range(0, 21).Select(_ => Guid.NewGuid()).ToList();
        }
    }
}
