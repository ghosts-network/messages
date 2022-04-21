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
    public async Task CreateNewChat_Ok()
    {
        // Arrange
        var model = new UpdateChatModel(It.IsAny<string>(), new List<string>());

        var chat = Chats.Chat.NewChat(model.Name, It.IsAny<List<UserInfo>>());

        var serviceMock = new Mock<IChatsService>();

        serviceMock
            .Setup(x => x.CreateAsync(chat.Name, It.IsAny<List<UserInfo>>()))
            .ReturnsAsync((DomainResult.Success(), chat.Id));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
        });

        // Act
        var response = await client.PostAsync("/chats", model.AsJsonContent());

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
    }
}