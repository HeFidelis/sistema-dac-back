namespace Ampara.Domain.Animals;

public sealed record AnimalFilter(string? Search, Species? Species, AnimalStatus? Status, int Page, int PageSize);

public interface IAnimalRepository
{
    Task<Animal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Animal> Items, int TotalCount)> ListAsync(AnimalFilter filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Animal>> ListAvailableAsync(Species? species, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<AnimalStatus, int>> CountByStatusAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DateOnly>> ListIntakeDatesSinceAsync(DateOnly since, CancellationToken cancellationToken = default);
    void Add(Animal animal);
}
