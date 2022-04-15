using System;
using System.Net;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Messages.Api.Controllers;
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
        //Setup
        var messageId = Guid.NewGuid();

        var model = new UpdateMessageModel("Upd");

        var serviceMock = new Mock<IMessageService>();
        var chatServiceMock = new Mock<IChatService>();
        
        serviceMock
            .Setup(x => x.UpdateMessageAsync(messageId, model.Message))
            .ReturnsAsync(DomainResult.Success);

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => chatServiceMock.Object);
        });
        
        //Act
        var response = await client.PutAsync($"/Chat/message/{messageId}", model.AsJsonContent());
        
        //Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Test]
    public async Task UpdateMessage_BadRequest()
    {
        //Setup
        var messageId = Guid.NewGuid();

        var model = new UpdateMessageModel("Upd");

        var serviceMock = new Mock<IMessageService>();
        var chatServiceMock = new Mock<IChatService>();
        
        serviceMock
            .Setup(x => x.UpdateMessageAsync(messageId, model.Message))
            .ReturnsAsync(DomainResult.Error(""));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => chatServiceMock.Object);
        });
        
        //Act
        var response = await client.PutAsync($"/Chat/message/{messageId}", model.AsJsonContent());
        
        //Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}