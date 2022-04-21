using System;
using System.Net;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Messages.Api.Controllers;
using GhostNetwork.Messages.Messages;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Messages.ApiTests.Messages;

[TestFixture]
public class PutMessageTests
{
    // Todo
    // [Test]
    // public async Task UpdateMessage_NoContent()
    // {
    //     // Arrange
    //     var model = new UpdateMessageModel(Guid.NewGuid(), "message");
    //     var chatId = Guid.NewGuid();
    //     var message = new Message(Guid.NewGuid().ToString(), chatId, It.IsAny<UserInfo>(), DateTimeOffset.Now, false, model.Message);
    //
    //     var userMock = new Mock<IUserProvider>();
    //     var serviceMock = new Mock<IMessagesService>();
    //
    //     serviceMock
    //         .Setup(x => x.UpdateAsync(It.IsAny<string>(), model.Message))
    //         .ReturnsAsync(DomainResult.Success);
    //
    //     var client = TestServerHelper.New(collection =>
    //     {
    //         collection.AddScoped(_ => serviceMock.Object);
    //         collection.AddScoped(_ => userMock.Object);
    //     });
    //
    //     // Act
    //     var response = await client.PutAsync($"/chats/messages/{message.Id}", model.AsJsonContent());
    //
    //     // Assert
    //     Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    // }
    //
    // [Test]
    // public async Task UpdateMessage_BadRequest()
    // {
    //     // Arrange
    //     var model = new UpdateMessageModel(Guid.NewGuid(), It.IsAny<string>());
    //     var chatId = Guid.NewGuid();
    //     var message = new Message("some_id", chatId, new UserInfo(model.SenderId, It.IsAny<string>(), It.IsAny<string>()), DateTimeOffset.Now, false, model.Message);
    //
    //     var userMock = new Mock<IUserProvider>();
    //     var serviceMock = new Mock<IMessagesService>();
    //
    //     serviceMock
    //         .Setup(x => x.UpdateAsync(message.Id, model.Message))
    //         .ReturnsAsync(DomainResult.Error(string.Empty));
    //
    //     var client = TestServerHelper.New(collection =>
    //     {
    //         collection.AddScoped(_ => serviceMock.Object);
    //         collection.AddScoped(_ => userMock.Object);
    //     });
    //
    //     // Act
    //     var response = await client.PutAsync($"/chats/messages/{message.Id}", model.AsJsonContent());
    //
    //     // Assert
    //     Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    // }
}