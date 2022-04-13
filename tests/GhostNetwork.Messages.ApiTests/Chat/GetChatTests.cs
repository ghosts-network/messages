using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
        //Setup
        IEnumerable<Guid> users = new[]
        {
            Guid.NewGuid(),
            Guid.Empty
        };

        var chat = GhostNetwork.Messages.Chat.NewChat(users);

        var serviceMock = new Mock<IChatService>();
        serviceMock
            .Setup(x => x.GetChatByIdAsync(chat.Id))
            .ReturnsAsync(chat);
        
        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
        });
        
        //Act
        var response = await client.GetAsync($"/Chat/{chat.Id}");
        
        //Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Test]
    public async Task SearchUserChats_Ok()
    {
        //Setup
        var skip = 0;
        var take = 1;
        var userId = Guid.NewGuid();
        
        var users = new[]
        {
            new GhostNetwork.Messages.Chat(default, default)
        };

        var serviceMock = new Mock<IChatService>();
        serviceMock
            .Setup(x => x.SearchChatsAsync(skip, take, userId))
            .ReturnsAsync((users.Select(x => x.Id), users.Length));
        
        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
        });
        
        //Act
        var response = await client.GetAsync($"/Chat/search/{userId}");
        
        //Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}