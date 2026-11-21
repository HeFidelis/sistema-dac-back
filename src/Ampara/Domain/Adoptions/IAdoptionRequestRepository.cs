namespace Ampara.Domain.Adoptions;

public interface IAdoptionRequestRepository
{
    /// <summary>Loads the request with its animal and adopter.</summary>
    Task<AdoptionRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<AdoptionRequest> Items, int TotalCount)> ListAsync(AdoptionStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdoptionRequest>> ListByAdopterAsync(Guid adopterId, CancellationToken cancellationToken = default);
    Task<bool> HasPendingAsync(Guid adopterId, Guid animalId, CancellationToken cancellationToken = default);
    Task<int> CountPendingAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DateTimeOffset>> ListApprovalDatesSinceAsync(DateTimeOffset since, CancellationToken cancellationToken = default);
    void Add(AdoptionRequest request);
}
