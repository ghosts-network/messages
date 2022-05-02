using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Messages.Chats;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Messages.ApiTests.Chat;

[TestFixture]
public class DeleteChatTests
{
    [Test]
    public async Task DeleteChat_NoContent()
    {
        // Arrange
        var chatId = Guid.NewGuid();

        var chat = new Chats.Chat(chatId, "Name", It.IsAny<IEnumerable<UserInfo>>());

        var chatsServiceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();

        chatsServiceMock
            .Setup(x => x.GetByIdAsync(chatId))
            .ReturnsAsync(chat);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.DeleteAsync($"/chats/{chatId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Test]
    public async Task DeleteChat_NotFound()
    {
        // Arrange
        var chatId = Guid.NewGuid();

        var chatsServiceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();

        chatsServiceMock
            .Setup(x => x.GetByIdAsync(chatId))
            .ReturnsAsync(default(Chats.Chat));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => chatsServiceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.DeleteAsync($"/chats/{chatId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}