using Ampara.Application.Auth;
using Ampara.Application.Common;
using Ampara.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ampara.Api.Controllers;

/// <summary>Team management ("Equipe" tab): only the SuperAdmin promotes or demotes people.</summary>
[ApiController]
[Route("api/users")]
[Authorize(Roles = Roles.SuperAdmin)]
public class UsersController(UserService users) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<UserResponse>> List([FromQuery] UserListQuery query, CancellationToken cancellationToken) =>
        users.ListAsync(query, cancellationToken);

    [HttpPut("{id:guid}/role")]
    public Task<UserResponse> ChangeRole(Guid id, ChangeRoleRequest request, CancellationToken cancellationToken) =>
        users.ChangeRoleAsync(id, request, cancellationToken);
}
