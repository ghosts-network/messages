﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Messages.Chats;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Messages.ApiTests.Chat;

[TestFixture]
public class GetChatTests
{
    [Test]
    public async Task GetChatInfo_Ok()
    {
        // Arrange
        var chat = Chats.Chat.NewChat(new Id(Guid.NewGuid().ToString()), "Test", new List<UserInfo>());

        var chatsServiceMock = new Mock<IChatsService>();

        chatsServiceMock
            .Setup(x => x.GetByIdAsync(chat.Id))
            .ReturnsAsync(chat);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
        });

        // Act
        var response = await client.GetAsync($"/chats/{chat.Id}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [Test]
    public async Task SearchUserChats_Ok()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var users = new[]
        {
            new Chats.Chat(default, default, default)
        };

        var chatsServiceMock = new Mock<IChatsService>();

        chatsServiceMock
            .Setup(x => x.SearchAsync(It.IsAny<ChatFilter>(), It.IsAny<Pagination>()))
            .ReturnsAsync(users);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
        });

        // Act
        var response = await client.GetAsync($"/chats/?userId={userId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}