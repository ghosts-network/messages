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
        //Setup
        var chatId = Guid.NewGuid();

        var model = new UpdateChatModel(It.IsAny<string>(), new List<Guid> {Guid.NewGuid()});

        var serviceMock = new Mock<IChatService>();

        serviceMock
            .Setup(x => x.UpdateAsync(chatId, model.Name, model.Users))
            .ReturnsAsync(DomainResult.Success());

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
        });
        
        //Act
        var response = await client.PutAsync($"/Chat/{chatId}", model.AsJsonContent());
        
        //Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Test]
    public async Task Update_BadRequest()
    {
        //Setup
        var chatId = Guid.NewGuid();

        var model = new UpdateChatModel(It.IsAny<string>(), new List<Guid> {Guid.NewGuid()});

        var serviceMock = new Mock<IChatService>();

        serviceMock
            .Setup(x => x.UpdateAsync(chatId, model.Name, model.Users))
            .ReturnsAsync(DomainResult.Error("Err"));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
        });
        
        //Act
        var response = await client.PutAsync($"/Chat/{chatId}", model.AsJsonContent());
        
        //Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}