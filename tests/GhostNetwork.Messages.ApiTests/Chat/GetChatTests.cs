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
public class GetChatTests
{
    [Test]
    public async Task GetChatInfo_Ok()
    {
        //Setup
        var chat = Chats.Chat.NewChat("Test", new List<Guid> { Guid.NewGuid() });

        var serviceMock = new Mock<IChatService>();

        serviceMock
            .Setup(x => x.GetByIdAsync(chat.Id))
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
            new Chats.Chat(default, default, default)
        };

        var serviceMock = new Mock<IChatService>();

        serviceMock
            .Setup(x => x.SearchAsync(skip, take, userId))
            .ReturnsAsync((users, users.Length));
        
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