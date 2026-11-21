using Ampara.Domain.Animals;
using Ampara.Domain.Common;
using static Ampara.UnitTests.TestData;

namespace Ampara.UnitTests.Domain;

public class AnimalTests
{
    [Fact]
    public void Register_WithValidProfile_TrimsNameAndKeepsStatus()
    {
        var animal = Animal.Register(Profile(name: "  Luna  "), AnimalStatus.InTreatment, Now);

        Assert.Equal("Luna", animal.Name);
        Assert.Equal(AnimalStatus.InTreatment, animal.Status);
        Assert.False(animal.IsAvailable);
    }

    [Fact]
    public void Register_AsAdopted_Throws()
    {
        Assert.Throws<DomainException>(() => Animal.Register(Profile(), AnimalStatus.Adopted, Now));
    }

    [Fact]
    public void Register_WithBlankName_Throws()
    {
        var error = Assert.Throws<DomainException>(() => Animal.Register(Profile(name: " "), AnimalStatus.Available, Now));

        Assert.Equal("O campo nome é obrigatório.", error.Message);
    }

    [Fact]
    public void Register_WithIntakeDateInTheFuture_Throws()
    {
        var tomorrow = DateOnly.FromDateTime(Now.UtcDateTime).AddDays(1);

        Assert.Throws<DomainException>(() => Animal.Register(Profile(intakeDate: tomorrow), AnimalStatus.Available, Now));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(Animal.MaxAgeMonths + 1)]
    public void Register_WithAgeOutOfRange_Throws(int ageMonths)
    {
        Assert.Throws<DomainException>(() => Animal.Register(Profile(ageMonths: ageMonths), AnimalStatus.Available, Now));
    }

    [Fact]
    public void Update_ChangesProfileAndStatus()
    {
        var animal = NewAnimal();

        animal.Update(Profile(name: "Luna Maria", ageMonths: 30), AnimalStatus.InTreatment, Now);

        Assert.Equal("Luna Maria", animal.Name);
        Assert.Equal(30, animal.ApproximateAgeMonths);
        Assert.Equal(AnimalStatus.InTreatment, animal.Status);
    }

    [Fact]
    public void Update_ToAdopted_Throws()
    {
        var animal = NewAnimal();

        Assert.Throws<DomainException>(() => animal.Update(Profile(), AnimalStatus.Adopted, Now));
    }

    [Fact]
    public void Update_AdoptedAnimalBackToAvailable_Throws()
    {
        var animal = NewAnimal();
        animal.MarkAsAdopted();

        Assert.Throws<DomainException>(() => animal.Update(Profile(), AnimalStatus.Available, Now));
    }

    [Fact]
    public void MarkAsAdopted_WhenAvailable_SetsAdopted()
    {
        var animal = NewAnimal();

        animal.MarkAsAdopted();

        Assert.Equal(AnimalStatus.Adopted, animal.Status);
    }

    [Fact]
    public void MarkAsAdopted_WhenInTreatment_Throws()
    {
        var animal = NewAnimal(AnimalStatus.InTreatment);

        Assert.Throws<DomainException>(animal.MarkAsAdopted);
    }
}
