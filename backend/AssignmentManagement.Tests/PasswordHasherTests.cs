using AssignmentManagement.Api.Services;

namespace AssignmentManagement.Tests;

public class PasswordHasherTests
{
    [Fact]
    public void HashAndVerifyUsesSaltedPbkdf2()
    {
        var hasher = new Pbkdf2PasswordHasher();

        var firstHash = hasher.Hash("SecurePassword123!");
        var secondHash = hasher.Hash("SecurePassword123!");

        Assert.NotEqual(firstHash, secondHash);
        Assert.True(hasher.Verify("SecurePassword123!", firstHash));
        Assert.False(hasher.Verify("WrongPassword", firstHash));
    }
}
