﻿using System;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Messages.Messages;
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
        // Arrange
        var chatId = Guid.NewGuid();
        var messageId = Guid.NewGuid().ToString();

        var serviceMock = new Mock<IMessagesService>();

        serviceMock
            .Setup(x => x.DeleteAsync(messageId));

        var client = TestServerHelper.New(collection =>
        {
            collection.AddScoped(_ => serviceMock.Object);
        });

        // Act
        var response = await client.DeleteAsync($"/chats/{chatId}/messages/{messageId}");

        // Assert
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }
}