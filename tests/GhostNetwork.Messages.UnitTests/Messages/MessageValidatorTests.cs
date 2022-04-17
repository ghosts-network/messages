﻿using System.Linq;
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

        // Act
        var result = validator.Validate(new MessageContext(null));

        // Assert
        Assert.IsFalse(result.Successed && result.Errors.Count() == 1);
    }

    [Test]
    public void Message_Correct_Data()
    {
        // Arrange
        var validator = new MessageValidator();

        // Act
        var result = validator.Validate(new MessageContext("Message"));

        // Assert
        Assert.IsTrue(result.Successed);
    }
}