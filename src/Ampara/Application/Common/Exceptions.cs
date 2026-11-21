namespace Ampara.Application.Common;

/// <summary>The resource does not exist or is not visible to the current user (HTTP 404).</summary>
public class NotFoundException(string message) : Exception(message);

/// <summary>Missing or invalid credentials (HTTP 401).</summary>
public class UnauthorizedException(string message) : Exception(message);

/// <summary>The request clashes with existing data, e.g. a duplicate e-mail (HTTP 409).</summary>
public class ConflictException(string message) : Exception(message);
