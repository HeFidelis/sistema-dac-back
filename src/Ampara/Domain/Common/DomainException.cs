namespace Ampara.Domain.Common;

/// <summary>
/// A business rule was violated. The message is shown to the user, so it is written in pt-BR.
/// </summary>
public class DomainException(string message) : Exception(message);
