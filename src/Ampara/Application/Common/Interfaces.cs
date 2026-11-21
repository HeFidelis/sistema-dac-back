using Ampara.Domain.Users;

namespace Ampara.Application.Common;

/// <summary>Who is calling the API, read from the JWT.</summary>
public interface ICurrentUser
{
    Guid? UserId { get; }
    UserRole? Role { get; }
}

public static class CurrentUserExtensions
{
    public static Guid RequireUserId(this ICurrentUser user) =>
        user.UserId ?? throw new UnauthorizedException("Faça login para continuar.");

    public static bool IsAdmin(this ICurrentUser user) => user.Role == UserRole.Admin;
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string passwordHash, string password);
}

public sealed record AccessToken(string Token, DateTimeOffset ExpiresAt);

public interface ITokenService
{
    AccessToken Create(User user);
}

public interface IFileStorage
{
    /// <returns>Public relative URL of the stored file, e.g. /uploads/animals/abc.jpg.</returns>
    Task<string> SaveAsync(Stream content, string folder, string fileName, CancellationToken cancellationToken = default);

    Task DeleteAsync(string url, CancellationToken cancellationToken = default);
}
