using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
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
        //Setup
        IEnumerable<Guid> users = new[]
        {
            Guid.NewGuid(),
            Guid.NewGuid()
        };

        var chat = GhostNetwork.Messages.Chat.NewChat(users);

        var serviceMock = new Mock<IChatService>();
        serviceMock
            .Setup(x => x.CreateNewChatAsync(users))
            .ReturnsAsync(chat.Id);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
        });
        
        //Act
        var response = await client.PostAsync($"/Chat", users.AsJsonContent());
        
        //Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}