﻿using System;
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

namespace GhostNetwork.Messages.ApiTests.Messages;

[TestFixture]
public class PostMessageTests
{
    [Test]
    public async Task SendMessage_Ok()
    {
        // Arrange
        var model = new CreateMessageModel(Guid.NewGuid(), "test");
        var p1 = new UserInfo(model.SenderId, "Test1", null);
        var p2 = new UserInfo(Guid.NewGuid(), "Test2", null);
        var chat = new Chat(ObjectId.GenerateNewId(), "Test", new[] { p1, p2 });

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        chatsStorageMock
            .Setup(x => x.GetByIdAsync(chat.Id))
            .ReturnsAsync(chat);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.PostAsync($"/chats/{chat.Id}/messages", model.AsJsonContent());

        // Assert
        messagesStorageMock.Verify(x => x.InsertAsync(It.IsAny<Message>()), Times.Once());
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
    }

    [Test]
    public async Task SendMessage_ChatNotFound_1()
    {
        // Arrange
        var model = new CreateMessageModel(Guid.NewGuid(), "test");
        var p1 = new UserInfo(model.SenderId, "Test1", null);
        var p2 = new UserInfo(Guid.NewGuid(), "Test2", null);
        var chat = new Chat(ObjectId.GenerateNewId(), "Test", new[] { p1, p2 });

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        chatsStorageMock
            .Setup(x => x.GetByIdAsync(chat.Id))
            .ReturnsAsync(default(Chat));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.PostAsync($"/chats/{chat.Id}/messages", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task SendMessage_ChatNotFound_2()
    {
        // Arrange
        var model = new CreateMessageModel(Guid.NewGuid(), "test");
        var p1 = new UserInfo(model.SenderId, "Test1", null);
        var p2 = new UserInfo(Guid.NewGuid(), "Test2", null);
        var chat = new Chat(ObjectId.GenerateNewId(), "Test", new[] { p1, p2 });

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        chatsStorageMock
            .Setup(x => x.GetByIdAsync(chat.Id))
            .ReturnsAsync(chat);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.PostAsync("/chats/invalid_id/messages", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task SendMessage_SenderNotInChat_BadRequest()
    {
        // Arrange
        var model = new CreateMessageModel(Guid.NewGuid(), "test");
        var p1 = new UserInfo(Guid.NewGuid(), "Test1", null);
        var p2 = new UserInfo(Guid.NewGuid(), "Test2", null);
        var chat = new Chat(ObjectId.GenerateNewId(), "Test", new[] { p1, p2 });

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        chatsStorageMock
            .Setup(x => x.GetByIdAsync(chat.Id))
            .ReturnsAsync(chat);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.PostAsync($"/chats/{chat.Id}/messages", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus")]
    public async Task SendMessage_InvalidContent_BadRequest(string content)
    {
        // Arrange
        var model = new CreateMessageModel(Guid.NewGuid(), content);
        var p1 = new UserInfo(model.SenderId, "Test1", null);
        var p2 = new UserInfo(Guid.NewGuid(), "Test2", null);
        var chat = new Chat(ObjectId.GenerateNewId(), "Test", new[] { p1, p2 });

        var chatsStorageMock = new Mock<IChatsStorage>();
        var messagesStorageMock = new Mock<IMessagesStorage>();

        chatsStorageMock
            .Setup(x => x.GetByIdAsync(chat.Id))
            .ReturnsAsync(chat);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsStorageMock.Object);
            collection.AddScoped(_ => messagesStorageMock.Object);
        });

        // Act
        var response = await client.PostAsync($"/chats/{chat.Id}/messages", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}