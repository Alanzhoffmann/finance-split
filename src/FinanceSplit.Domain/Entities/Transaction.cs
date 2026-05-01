using FinanceSplit.Domain.ValueObjects;

namespace FinanceSplit.Domain.Entities;

public class Transaction : BaseEntity
{
    // Required for EF Core materialization
    private Transaction() { }

    public Transaction(string title, decimal amount, Person paidBy, SplitPay? splitPay = null, DateTime? date = null, Recurrence? recurrence = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(paidBy);

        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");
        }

        Title = title;
        Amount = amount;
        PaidBy = paidBy;
        Date = date ?? DateTime.UtcNow;
        Recurrence = recurrence;

        if (splitPay is not null)
        {
            SplitPay = splitPay;
            _participants = splitPay.People.ToList();
        }
    }

    private List<Person> _participants = [];

    /// <summary>
    /// Navigation property for EF Core to persist split participants.
    /// </summary>
    public IReadOnlyCollection<Person> Participants => _participants;

    public string Title { get; private set; } = string.Empty;
    public Person PaidBy { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public Recurrence? Recurrence { get; private set; }

    public void Update(string title, decimal amount, Person paidBy, SplitPay splitPay, DateTime? date = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(paidBy);

        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");
        }

        Title = title;
        Amount = amount;
        PaidBy = paidBy;
        Date = date ?? Date;
        SplitPay = splitPay;
        _participants = splitPay.People.ToList();
    }

    public SplitPay SplitPay
    {
        get
        {
            if (field is not null && field.People is not null)
            {
                return field;
            }

            // Reconstruct from stored SplitType + Participants (EF ignores People)
            if (field is not null && _participants.Count > 0)
            {
                field = new SplitPay(_participants, field.SplitType);
            }
            else if (_participants.Count > 0)
            {
                field = SplitPay.CreateNoneSplit(_participants[0]);
            }
            else
            {
                field = SplitPay.CreateNoneSplit(PaidBy);
            }

            return field;
        }
        private set;
    }
}
