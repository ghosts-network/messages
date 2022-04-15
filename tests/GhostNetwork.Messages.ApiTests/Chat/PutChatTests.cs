using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Messages.ApiTests.Chat;

[TestFixture]
public class PutChatTests
{
    [Test]
    public async Task AddNewUsersToChat_NoContent()
    {
        //Setup
        var chatId = Guid.NewGuid();

        IEnumerable<Guid> users = new[]
        {
            Guid.NewGuid(),
            Guid.NewGuid()
        };

        var serviceMock = new Mock<IChatService>();
        var messageServiceMock = new Mock<IMessageService>();
        
        serviceMock
            .Setup(x => x.AddNewUsersToChatAsync(chatId, users));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => messageServiceMock.Object);
        });
        
        //Act
        var response = await client.PutAsync($"/Chat/{chatId}", users.AsJsonContent());
        
        //Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }
}