using Ampara.Domain.Adoptions;
using Ampara.Domain.Animals;
using Ampara.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Ampara.Infrastructure.Persistence;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default) =>
        db.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public void Add(User user) => db.Users.Add(user);
}

public class AnimalRepository(AppDbContext db) : IAnimalRepository
{
    public Task<Animal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Animals.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Animal> Items, int TotalCount)> ListAsync(AnimalFilter filter, CancellationToken cancellationToken = default)
    {
        var query = db.Animals.AsNoTracking();

        if (!string.IsNullOrEmpty(filter.Search))
            query = query.Where(a => EF.Functions.ILike(a.Name, $"%{filter.Search}%"));

        if (filter.Species is { } species)
            query = query.Where(a => a.Species == species);

        if (filter.Status is { } status)
            query = query.Where(a => a.Status == status);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(a => a.IntakeDate).ThenBy(a => a.Name)
            .Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IReadOnlyList<Animal>> ListAvailableAsync(Species? species, CancellationToken cancellationToken = default)
    {
        var query = db.Animals.AsNoTracking().Where(a => a.Status == AnimalStatus.Available);

        if (species is { } value)
            query = query.Where(a => a.Species == value);

        return await query.OrderByDescending(a => a.IntakeDate).ThenBy(a => a.Name).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<AnimalStatus, int>> CountByStatusAsync(CancellationToken cancellationToken = default) =>
        await db.Animals
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);

    public async Task<IReadOnlyList<DateOnly>> ListIntakeDatesSinceAsync(DateOnly since, CancellationToken cancellationToken = default) =>
        await db.Animals.Where(a => a.IntakeDate >= since).Select(a => a.IntakeDate).ToListAsync(cancellationToken);

    public void Add(Animal animal) => db.Animals.Add(animal);
}

public class AdoptionRequestRepository(AppDbContext db) : IAdoptionRequestRepository
{
    private IQueryable<AdoptionRequest> WithDetails => db.AdoptionRequests.Include(r => r.Animal).Include(r => r.Adopter);

    public Task<AdoptionRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        WithDetails.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<AdoptionRequest> Items, int TotalCount)> ListAsync(
        AdoptionStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = WithDetails.AsNoTracking();

        if (status is { } value)
            query = query.Where(r => r.Status == value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IReadOnlyList<AdoptionRequest>> ListByAdopterAsync(Guid adopterId, CancellationToken cancellationToken = default) =>
        await WithDetails.AsNoTracking()
            .Where(r => r.AdopterId == adopterId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task<bool> HasPendingAsync(Guid adopterId, Guid animalId, CancellationToken cancellationToken = default) =>
        db.AdoptionRequests.AnyAsync(
            r => r.AdopterId == adopterId && r.AnimalId == animalId && r.Status == AdoptionStatus.Pending, cancellationToken);

    public Task<int> CountPendingAsync(CancellationToken cancellationToken = default) =>
        db.AdoptionRequests.CountAsync(r => r.Status == AdoptionStatus.Pending, cancellationToken);

    public async Task<IReadOnlyList<DateTimeOffset>> ListApprovalDatesSinceAsync(DateTimeOffset since, CancellationToken cancellationToken = default) =>
        await db.AdoptionRequests
            .Where(r => r.Status == AdoptionStatus.Approved && r.DecidedAt >= since)
            .Select(r => r.DecidedAt!.Value)
            .ToListAsync(cancellationToken);

    public void Add(AdoptionRequest request) => db.AdoptionRequests.Add(request);
}
