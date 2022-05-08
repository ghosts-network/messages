using System;
using System.Collections.Generic;
using System.Linq;
using GhostNetwork.Messages.Chats;
using NUnit.Framework;

namespace GhostNetwork.Messages.UnitTests.Chats;

public class ChatValidatorTests
{
    [Test]
    public void Name_Null_Argument()
    {
        // Arrange
        var validator = new ChatValidator();

        // Act
        var result = validator.Validate(new Chat(new Id(Guid.NewGuid().ToString()), "test", new List<UserInfo>() { new(Guid.NewGuid(), "Name", null) }));

        // Assert
        Assert.IsFalse(result.Successed && result.Errors.Count() == 1);
    }

    [Test]
    public void Chat_Correct_Data()
    {
        // Arrange
        var validator = new ChatValidator();

        // Act
        var result = validator.Validate(new Chat(new Id(Guid.NewGuid().ToString()), "Test", new List<UserInfo> { new(Guid.NewGuid(), "Name", null) }));

        // Assert
        Assert.IsTrue(result.Successed);
    }

    [Test]
    public void Chat_Users_Zero_Argument()
    {
        // Arrange
        var validator = new ChatValidator();

        // Act
        var result = validator.Validate(new Chat(new Id(Guid.NewGuid().ToString()), "Test", new List<UserInfo>()));

        // Assert
        Assert.IsFalse(result.Successed && result.Errors.Count() == 1);
    }

    [Test]
    public void Chat_Contains_Duplicate_Users()
    {
        // Arrange
        var validator = new ChatValidator();

        var userId = Guid.NewGuid();

        var users = new List<UserInfo>()
        {
            new UserInfo(userId, "Name", null),
            new UserInfo(userId, "Name1", null)
        };

        // Act
        var result = validator.Validate(new Chat(new Id(Guid.NewGuid().ToString()), "Test", users));

        // Assert
        Assert.IsFalse(result.Successed && result.Errors.Count() == 1);
    }
}