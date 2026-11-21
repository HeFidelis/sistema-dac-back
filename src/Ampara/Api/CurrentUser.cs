using Ampara.Application.Common;
using Ampara.Domain.Users;
using Ampara.Infrastructure.Security;

namespace Ampara.Api;

public class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid? UserId =>
        Guid.TryParse(accessor.HttpContext?.User.FindFirst(JwtClaims.Subject)?.Value, out var id) ? id : null;

    public UserRole? Role =>
        Enum.TryParse<UserRole>(accessor.HttpContext?.User.FindFirst(JwtClaims.Role)?.Value, out var role) ? role : null;
}
