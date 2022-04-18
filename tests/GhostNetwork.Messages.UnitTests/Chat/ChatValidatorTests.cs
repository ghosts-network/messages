using System;
using System.Collections.Generic;
using System.Linq;
using GhostNetwork.Messages.Chats;
using NUnit.Framework;

namespace GhostNetwork.Messages.UnitTests.Chat;

public class ChatValidatorTests
{
    [Test]
    public void Name_Null_Argument()
    {
        // Arrange
        var validator = new ChatValidator();

        // Act
        var result = validator.Validate(new ChatContext(null, new List<Guid>() { Guid.NewGuid() }));

        // Assert
        Assert.IsFalse(result.Successed && result.Errors.Count() == 1);
    }

    [Test]
    public void Chat_Correct_Data()
    {
        // Arrange
        var validator = new ChatValidator();

        // Act
        var result = validator.Validate(new ChatContext("Test", new List<Guid> { Guid.NewGuid() }));

        // Assert
        Assert.IsTrue(result.Successed);
    }

    [Test]
    public void Chat_Users_Zero_Argument()
    {
        // Arrange
        var validator = new ChatValidator();

        // Act
        var result = validator.Validate(new ChatContext("Test", new List<Guid>()));

        // Assert
        Assert.IsFalse(result.Successed && result.Errors.Count() == 1);
    }
    
    [Test]
    public void Chat_Contains_Duplicate_Users()
    {
        //Setup
        var validator = new ChatValidator();

        var userId = Guid.NewGuid();

        var users = new List<Guid>()
        {
            userId,
            userId
        };
        
        //Act
        var result = validator.Validate(new ChatContext("Test", users));
        
        //Assert
        Assert.IsFalse(result.Successed && result.Errors.Count() == 1);
    }
}