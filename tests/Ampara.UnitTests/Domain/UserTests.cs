using Ampara.Domain.Common;
using Ampara.Domain.Users;
using static Ampara.UnitTests.TestData;

namespace Ampara.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Register_NormalizesEmailToLowerCase()
    {
        var user = User.Register(" Ana Souza ", "Ana.Souza@Exemplo.com", null, UserRole.Adopter, "hash", Now);

        Assert.Equal("ana.souza@exemplo.com", user.Email);
        Assert.Equal("Ana Souza", user.FullName);
        Assert.Null(user.Phone);
    }

    [Fact]
    public void Register_WithInvalidEmail_Throws()
    {
        var error = Assert.Throws<DomainException>(() => User.Register("Ana", "ana.exemplo.com", null, UserRole.Adopter, "hash", Now));

        Assert.Equal("O campo e-mail não é um e-mail válido.", error.Message);
    }

    [Fact]
    public void Register_WithBlankName_Throws()
    {
        Assert.Throws<DomainException>(() => User.Register("", "ana@exemplo.com", null, UserRole.Adopter, "hash", Now));
    }
}
