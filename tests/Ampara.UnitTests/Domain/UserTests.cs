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

    [Fact]
    public void ChangeRole_PromotesAdopterAndDemotesBack()
    {
        var user = NewAdopter();

        user.ChangeRole(UserRole.Admin);
        Assert.Equal(UserRole.Admin, user.Role);

        user.ChangeRole(UserRole.Adopter);
        Assert.Equal(UserRole.Adopter, user.Role);
    }

    [Fact]
    public void ChangeRole_ToSuperAdmin_Throws()
    {
        var error = Assert.Throws<DomainException>(() => NewAdopter().ChangeRole(UserRole.SuperAdmin));

        Assert.Equal("Não é possível promover alguém a super admin.", error.Message);
    }

    [Fact]
    public void ChangeRole_OfTheSuperAdmin_Throws()
    {
        var owner = User.Register("Mariana Costa", "admin@ampara.local", null, UserRole.SuperAdmin, "hash", Now);

        Assert.Throws<DomainException>(() => owner.ChangeRole(UserRole.Adopter));
        Assert.Equal(UserRole.SuperAdmin, owner.Role);
    }
}
