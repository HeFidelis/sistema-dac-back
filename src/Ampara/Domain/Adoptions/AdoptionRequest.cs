using Ampara.Domain.Animals;
using Ampara.Domain.Common;
using Ampara.Domain.Users;

namespace Ampara.Domain.Adoptions;

public enum AdoptionStatus { Pending, Approved, Rejected }

// What the adopter tells the NGO in the adoption form.
public sealed record ApplicantDetails(string Phone, string Housing, string Routine, string? Message);

public class AdoptionRequest : Entity
{
    public const int PhoneMaxLength = 30;
    public const int TextMaxLength = 1000;
    public const int ReasonMaxLength = 500;

    public Guid AnimalId { get; private set; }
    public Animal Animal { get; private set; } = null!;
    public Guid AdopterId { get; private set; }
    public User Adopter { get; private set; } = null!;
    public string Phone { get; private set; } = string.Empty;
    public string Housing { get; private set; } = string.Empty;
    public string Routine { get; private set; } = string.Empty;
    public string? Message { get; private set; }
    public AdoptionStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? DecidedAt { get; private set; }

    private AdoptionRequest() { }

    public static AdoptionRequest Submit(Animal animal, User adopter, ApplicantDetails details, bool acceptsTerms, DateTimeOffset now)
    {
        if (!animal.IsAvailable)
            throw new DomainException("Este animal não está disponível para adoção no momento.");

        if (!acceptsTerms)
            throw new DomainException("Confirme que tem 18 anos ou mais e concorda com a análise da ONG.");

        return new AdoptionRequest
        {
            AnimalId = animal.Id,
            Animal = animal,
            AdopterId = adopter.Id,
            Adopter = adopter,
            Phone = Guard.Required(details.Phone, "telefone", PhoneMaxLength),
            Housing = Guard.Required(details.Housing, "moradia", TextMaxLength),
            Routine = Guard.Required(details.Routine, "rotina", TextMaxLength),
            Message = Guard.Optional(details.Message, "mensagem", TextMaxLength),
            Status = AdoptionStatus.Pending,
            CreatedAt = now,
        };
    }

    // Approves the request and marks the animal as adopted.
    public void Approve(DateTimeOffset now)
    {
        EnsurePending();
        Animal.MarkAsAdopted();
        Status = AdoptionStatus.Approved;
        DecidedAt = now;
    }

    public void Reject(string reason, DateTimeOffset now)
    {
        EnsurePending();
        RejectionReason = Guard.Required(reason, "motivo", ReasonMaxLength);
        Status = AdoptionStatus.Rejected;
        DecidedAt = now;
    }

    private void EnsurePending()
    {
        if (Status != AdoptionStatus.Pending)
            throw new DomainException("Esta solicitação já foi decidida.");
    }
}
