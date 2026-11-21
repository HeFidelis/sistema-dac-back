using Ampara.Domain.Common;

namespace Ampara.Domain.Users;

/// <summary>
/// SuperAdmin: the NGO owner (only one, from the seed), who also manages the team.
/// Admin: promoted by the SuperAdmin; runs animals, adoptions and the dashboard.
/// Adopter: anyone who signs up.
/// </summary>
public enum UserRole
{
    Admin,
    Adopter,
    SuperAdmin,
}

public class User : Entity
{
    public const int NameMaxLength = 120;
    public const int EmailMaxLength = 160;
    public const int PhoneMaxLength = 30;

    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public UserRole Role { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }

    private User() { }

    /// <param name="passwordHash">Already hashed; the plain password never reaches the domain.</param>
    public static User Register(string fullName, string email, string? phone, UserRole role, string passwordHash, DateTimeOffset now)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        return new User
        {
            FullName = Guard.Required(fullName, "nome completo", NameMaxLength),
            Email = Guard.RequiredEmail(email, "e-mail", EmailMaxLength).ToLowerInvariant(),
            Phone = Guard.Optional(phone, "telefone", PhoneMaxLength),
            Role = role,
            PasswordHash = passwordHash,
            CreatedAt = now,
        };
    }

    /// <summary>Promotes an adopter to the team (Admin) or moves an Admin back to adopter.</summary>
    public void ChangeRole(UserRole role)
    {
        if (role == UserRole.SuperAdmin)
            throw new DomainException("Não é possível promover alguém a super admin.");

        if (Role == UserRole.SuperAdmin)
            throw new DomainException("O papel do super admin não pode ser alterado.");

        Role = role;
    }
}
