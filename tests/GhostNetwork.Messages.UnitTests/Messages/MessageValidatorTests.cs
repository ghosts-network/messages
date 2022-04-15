using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GhostNetwork.Messages.UnitTests.Messages;

public class MessageValidatorTests
{
    [Test]
    public void Message_Null_Argument()
    {
        //Setup
        var validator = new MessageValidator();
        
        //Act
        var result = validator.Validate(new MessageContext(null));
        
        //Assert
        Assert.IsFalse(result.Successed && result.Errors.Count() == 1);
    }

    [Test]
    public void Message_Correct_Data()
    {
        //Setup
        var validator = new MessageValidator();
        
        //Act
        var result = validator.Validate(new MessageContext("Message"));
        
        //Assert
        Assert.IsTrue(result.Successed);
    }
}