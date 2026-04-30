using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.Enums;

namespace FinanceSplit.Domain.ValueObjects;

public class SplitPayTests
{
    [Test]
    public async Task CreateNoneSplit_ShouldCreateNoneSplit()
    {
        // Arrange
        var person = new Person("John");

        // Act
        var splitPay = SplitPay.CreateNoneSplit(person);

        // Assert
        await Assert
            .That(splitPay)
            .IsNotNull()
            .And.Member(s => s.People, p => p.IsNotNull().And.IsSingleElement().And.Contains(person))
            .And.Member(s => s.SplitType, st => st.IsEqualTo(SplitType.None));
    }
}
