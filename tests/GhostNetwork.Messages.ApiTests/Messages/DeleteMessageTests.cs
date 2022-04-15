using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Messages.ApiTests.Messages;

[TestFixture]
public class DeleteMessageTests
{
    [Test]
    public async Task DeleteMessage_NoContent()
    {
        //Setup
        var messageId = Guid.NewGuid();

        var chatServiceMock = new Mock<IChatService>();
        var serviceMock = new Mock<IMessageService>();
        serviceMock
            .Setup(x => x.DeleteMessageAsync(messageId));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
            collection.AddScoped(_ => chatServiceMock.Object);
        });
        
        //Act
        var response = await client.DeleteAsync($"/Chat/message/{messageId}");
        
        //Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }
}