using Ampara.Domain.Users;

namespace Ampara.Application.Common;

/// <summary>Role names as written in the JWT, for [Authorize(Roles = ...)].</summary>
public static class Roles
{
    public const string SuperAdmin = nameof(UserRole.SuperAdmin);
    public const string Admin = nameof(UserRole.Admin);
    public const string Adopter = nameof(UserRole.Adopter);

    /// <summary>The NGO team: animals, adoptions and dashboard.</summary>
    public const string Staff = $"{Admin},{SuperAdmin}";
}
