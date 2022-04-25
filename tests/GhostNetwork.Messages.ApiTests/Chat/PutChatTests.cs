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
        var model = new UpdateChatModel("Name", new List<string>());

        var chat = new Chats.Chat(chatId, model.Name, It.IsAny<List<UserInfo>>());

        var serviceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();

        serviceMock
            .Setup(x => x.UpdateAsync(chatId, model.Name, It.IsAny<List<UserInfo>>()))
            .ReturnsAsync(DomainResult.Success());

        serviceMock
            .Setup(x => x.GetByIdAsync(chatId))
            .ReturnsAsync(chat);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chatId}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Test]
    public async Task Update_BadRequest()
    {
        // Arrange
        var chatId = Guid.NewGuid();

        var model = new UpdateChatModel("Name", new List<string>());

        var serviceMock = new Mock<IChatsService>();
        var userServiceMock = new Mock<IUserProvider>();

        serviceMock
            .Setup(x => x.UpdateAsync(chatId, model.Name, It.IsAny<List<UserInfo>>()))
            .ReturnsAsync(DomainResult.Error("Err"));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => userServiceMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chatId}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}