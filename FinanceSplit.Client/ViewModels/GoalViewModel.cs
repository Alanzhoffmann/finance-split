using FinanceSplit.Contracts.Dashboard.DTOs;

namespace FinanceSplit.Client.ViewModels;

public class GoalViewModel
{
    public GoalViewModel(GoalDto dto)
    {
        Title = dto.Name;
        Month = dto.Month;
        Amount = dto.Amount;
        People =
        [
            .. dto.SharedWith.Select(item => new GoalPersonViewModel(
                item.Person.Name,
                new(
                    item.Amount,
                    dto.Completed.FirstOrDefault(c => c.Person.Id == item.Person.Id)?.Amount ?? 0
                )
            )),
        ];
    }

    public GoalViewModel() { }

    public string Title { get; set; }
    public DateOnly Month { get; set; }
    public decimal Amount { get; set; }
    public ProgressViewModel Progress => new(Amount, People.Sum(p => p.Progress.Completed));
    public IReadOnlyList<GoalPersonViewModel> People { get; set; } = [];
}

public record GoalPersonViewModel(string Name, ProgressViewModel Progress);

public record ProgressViewModel(decimal Amount, decimal Completed)
{
    public int Percentage => (int)Math.Floor(Completed / Amount * 100);
}
