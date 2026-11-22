using Ampara.Domain.Users;

namespace Ampara.Application.Common;

// Role names as written in the JWT, for [Authorize(Roles = ...)].
public static class Roles
{
    public const string SuperAdmin = nameof(UserRole.SuperAdmin);
    public const string Admin = nameof(UserRole.Admin);
    public const string Adopter = nameof(UserRole.Adopter);

    // The NGO team: animals, adoptions and dashboard.
    public const string Staff = $"{Admin},{SuperAdmin}";
}
