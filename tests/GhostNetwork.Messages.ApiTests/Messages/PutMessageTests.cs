using System;
using System.Net;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Messages.Api.Controllers;
using GhostNetwork.Messages.Chats;
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
        var chatId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        var model = new UpdateMessageModel("Upd");

        var serviceMock = new Mock<IMessagesService>();
        var chatServiceMock = new Mock<IChatsService>();

        serviceMock
            .Setup(x => x.UpdateAsync(messageId, model.Message))
            .ReturnsAsync(DomainResult.Success);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => chatServiceMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chatId}/messages/{messageId}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Test]
    public async Task UpdateMessage_BadRequest()
    {
        // Arrange
        var chatId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        var model = new UpdateMessageModel("Upd");

        var serviceMock = new Mock<IMessagesService>();
        var chatServiceMock = new Mock<IChatsService>();

        serviceMock
            .Setup(x => x.UpdateAsync(messageId, model.Message))
            .ReturnsAsync(DomainResult.Error(string.Empty));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => chatServiceMock.Object);
        });

        // Act
        var response = await client.PutAsync($"/chats/{chatId}/messages/{messageId}", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}