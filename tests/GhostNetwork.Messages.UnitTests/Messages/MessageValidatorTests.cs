using System;
using System.Collections.Generic;
using System.Linq;
using GhostNetwork.Messages.Messages;
using NUnit.Framework;

namespace GhostNetwork.Messages.UnitTests.Messages;

public class MessageValidatorTests
{
    [Test]
    public void Message_Null_Argument()
    {
        // Arrange
        var validator = new MessageValidator();
        var authorId = Guid.NewGuid();
        var participants = new List<Guid> { authorId };

        // Act
        var result = validator.Validate(new MessageContext(null, authorId, participants));

        // Assert
        Assert.IsFalse(result.Successed && result.Errors.Count() == 1);
    }

    [Test]
    public void Message_Correct_Data()
    {
        // Arrange
        var validator = new MessageValidator();
        var authorId = Guid.NewGuid();
        var participants = new List<Guid> { authorId };

        // Act
        var result = validator.Validate(new MessageContext("Message", authorId, participants));

        // Assert
        Assert.IsTrue(result.Successed);
    }

    [Test]
    public void Sender_IsNot_Author()
    {
        // Arrange
        var validator = new MessageValidator();
        var authorId = Guid.NewGuid();
        var participants = new List<Guid> { authorId };

        // Act
        var result = validator.Validate(new MessageContext("Message", Guid.NewGuid(), participants));

        // Assert
        Assert.IsFalse(result.Successed && result.Errors.Count() == 1);
    }
}