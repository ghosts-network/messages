using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Messages.Api.Controllers;
using GhostNetwork.Messages.Api.Domain;
using GhostNetwork.Messages.Chats;
using GhostNetwork.Messages.Users;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Messages.ApiTests.Chats;

[TestFixture]
public class PutChatTests
{
    [Test]
    public async Task Update_NoContent()
    {
        // Arrange
        var p1 = new UserInfo(Guid.NewGuid(), "Test1", null);
        var p2 = new UserInfo(Guid.NewGuid(), "Test2", null);
        var p3 = new UserInfo(Guid.NewGuid(), "Test3", null);
        var chat = new Chat(ObjectId.GenerateNewId(), "Test", new[] { p1, p2 });
        var model = new UpdateChatModel("Chat name", new List<Guid> { p1.Id, p2.Id, p3.Id });

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        chatsStorageMock
            .Setup(x => x.GetByIdAsync(chat.Id))
            .ReturnsAsync(chat);

        userStorageMock
            .Setup(x => x.SearchAsync(new List<Guid> { p1.Id, p2.Id, p3.Id }))
            .ReturnsAsync(new List<UserInfo> { p1, p2, p3 });

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chat.Id}/", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Test]
    public async Task Update_NotFound_1()
    {
        // Arrange
        var p1 = new UserInfo(Guid.NewGuid(), "Test1", null);
        var p2 = new UserInfo(Guid.NewGuid(), "Test2", null);
        var p3 = new UserInfo(Guid.NewGuid(), "Test3", null);
        var chat = new Chat(ObjectId.GenerateNewId(), "Test", new[] { p1, p2 });
        var model = new UpdateChatModel("Chat name", new List<Guid> { p1.Id, p2.Id, p3.Id });

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        chatsStorageMock
            .Setup(x => x.GetByIdAsync(chat.Id))
            .ReturnsAsync(chat);

        userStorageMock
            .Setup(x => x.SearchAsync(new List<Guid> { p1.Id, p2.Id, p3.Id }))
            .ReturnsAsync(new List<UserInfo> { p1, p2, p3 });

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.PutAsync("/chats/invalid_id/", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task Update_NotFound_2()
    {
        // Arrange
        var p1 = new UserInfo(Guid.NewGuid(), "Test1", null);
        var p2 = new UserInfo(Guid.NewGuid(), "Test2", null);
        var p3 = new UserInfo(Guid.NewGuid(), "Test3", null);
        var chat = new Chat(ObjectId.GenerateNewId(), "Test", new[] { p1, p2 });
        var model = new UpdateChatModel("Chat name", new List<Guid> { p1.Id, p2.Id, p3.Id });

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        chatsStorageMock
            .Setup(x => x.GetByIdAsync(chat.Id))
            .ReturnsAsync(default(Chat));

        userStorageMock
            .Setup(x => x.SearchAsync(new List<Guid> { p1.Id, p2.Id, p3.Id }))
            .ReturnsAsync(new List<UserInfo> { p1, p2, p3 });

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chat.Id}/", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("0123456789012345678900123456789001234567890012345678901", Description = "Too long name")]
    public async Task Update_InvalidName_BadRequest(string name)
    {
        // Arrange
        var p1 = new UserInfo(Guid.NewGuid(), "Test1", null);
        var p2 = new UserInfo(Guid.NewGuid(), "Test2", null);
        var p3 = new UserInfo(Guid.NewGuid(), "Test3", null);
        var chat = new Chat(ObjectId.GenerateNewId(), "Test", new[] { p1, p2 });
        var model = new UpdateChatModel(name, new List<Guid> { p1.Id, p2.Id, p3.Id });

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        chatsStorageMock
            .Setup(x => x.GetByIdAsync(chat.Id))
            .ReturnsAsync(chat);

        userStorageMock
            .Setup(x => x.SearchAsync(new List<Guid> { p1.Id, p2.Id, p3.Id }))
            .ReturnsAsync(new List<UserInfo> { p1, p2, p3 });

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chat.Id}/", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestCaseSource(typeof(ParticipantCases))]
    public async Task Update_InvalidParticipants_BadRequests(List<Guid> participants)
    {
        // Arrange
        var p1 = new UserInfo(Guid.NewGuid(), "Test1", null);
        var p2 = new UserInfo(Guid.NewGuid(), "Test2", null);
        var p3 = new UserInfo(Guid.NewGuid(), "Test3", null);
        var chat = new Chat(ObjectId.GenerateNewId(), "Test", new[] { p1, p2 });
        var model = new UpdateChatModel("Chat name", participants);

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        chatsStorageMock
            .Setup(x => x.GetByIdAsync(chat.Id))
            .ReturnsAsync(chat);

        userStorageMock
            .Setup(x => x.SearchAsync(participants))
            .ReturnsAsync(new List<UserInfo> { p1, p2, p3 });

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chat.Id}/", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Test]
    public async Task Update_SomeParticipantsNotExists_BadRequest()
    {
        // Arrange
        var p1 = new UserInfo(Guid.NewGuid(), "Test1", null);
        var p2 = new UserInfo(Guid.NewGuid(), "Test2", null);
        var p3 = new UserInfo(Guid.NewGuid(), "Test3", null);
        var chat = new Chat(ObjectId.GenerateNewId(), "Test", new[] { p1, p2 });
        var model = new UpdateChatModel("Chat name", new List<Guid> { p1.Id, p2.Id, p3.Id });

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();
        var userStorageMock = new Mock<IUsersStorage>();

        chatsStorageMock
            .Setup(x => x.GetByIdAsync(chat.Id))
            .ReturnsAsync(chat);

        userStorageMock
            .Setup(x => x.SearchAsync(new List<Guid> { p1.Id, p2.Id, p3.Id }))
            .ReturnsAsync(new List<UserInfo> { p1, p2 });

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
            collection.AddScoped(_ => userStorageMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chat.Id}/", model.AsJsonContent());

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