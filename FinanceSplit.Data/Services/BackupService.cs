using System.Text.Json;
using System.Text.Json.Serialization;
using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Services;
using FinanceSplit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Data.Services;

internal partial class BackupService(AppDbContext context, TimeProvider timeProvider)
    : IBackupService
{
    public async Task<BackupResponse> GetBackupAsync()
    {
        var people = await context
            .People.AsNoTracking()
            .OrderBy(p => p.Name.ToLower())
            .Select(p => new BackupPerson(
                p.Id,
                p.Name,
                p.IncomeRecords.OrderBy(s => s.Month)
                    .Select(s => new BackupIncomeRecord(s.Id, s.Month, s.Amount, s.Type))
                    .ToList()
            ))
            .ToListAsync();

        var transactions = await context
            .Transactions.AsNoTracking()
            .OrderBy(t => t.Month)
            .ThenBy(t => t.Name.ToLower())
            .Select(t => new BackupTransaction(
                t.Id,
                t.Name,
                t.Value,
                t.Month,
                t.SplitType,
                t.PaidBy.Id,
                t.SharedWith.OrderBy(r => r.Id).Select(r => r.Id).ToList()
            ))
            .ToListAsync();

        return new BackupResponse(
            Backup.Version,
            timeProvider.GetUtcNow(),
            new BackupPayload(people, transactions)
        );
    }

    public async Task<BackupImportResponse?> ImportBackupAsync(
        Stream file,
        string contentType,
        string fileName,
        CancellationToken cancellationToken = default
    )
    {
        var jsonFile = await JsonSerializer.DeserializeAsync(
            file,
            BackupSerializerContext.Default.BackupObject,
            cancellationToken
        );
        return jsonFile is null
            ? null
            : jsonFile switch
            {
                BackupLegacy legacy => await HandleAsync(legacy, cancellationToken),
                Backup backup => await HandleAsync(backup, cancellationToken),
                _ => throw new InvalidOperationException("Can't handle backup"),
            };
    }

    private async Task<BackupImportResponse> HandleAsync(
        BackupLegacy legacy,
        CancellationToken cancellationToken = default
    )
    {
        var existingPeople = await context.People.ToListAsync(cancellationToken: cancellationToken);
        var peopleMapping = new Dictionary<Guid, Person>();

        foreach (var person in legacy.Data.People)
        {
            var databasePerson =
                existingPeople.FirstOrDefault(p => p.Name == person.Name)
                ?? new Person() { Name = person.Name };
            peopleMapping[person.Id] = databasePerson;

            foreach (var monthSalaries in person.Salaries.GroupBy(s => (s.Year, s.Month)))
            {
                var salaryMonth = new DateOnly(monthSalaries.Key.Year, monthSalaries.Key.Month, 1);
                var existingIncomes = databasePerson
                    .IncomeRecords.Where(i => i.Month == salaryMonth)
                    .ToDictionary(i => i.Amount);

                var isSalary = true;
                foreach (var salary in monthSalaries.OrderByDescending(s => s.Amount))
                {
                    if (!existingIncomes.ContainsKey(salary.Amount))
                    {
                        databasePerson.IncomeRecords.Add(
                            new()
                            {
                                Month = salaryMonth,
                                Amount = salary.Amount,
                                Type = isSalary ? IncomeType.Salary : IncomeType.Extra,
                            }
                        );
                    }

                    isSalary = false;
                }
            }
        }

        var existingTransactions = await context.Transactions.ToDictionaryAsync(
            t => (t.Name, t.Month.Month, t.PaidBy.Id, t.Month.Year),
            cancellationToken: cancellationToken
        );

        foreach (var transaction in legacy.Data.Transactions)
        {
            if (
                existingTransactions.TryGetValue(
                    (
                        transaction.Name,
                        transaction.Month,
                        transaction.PaidByPersonId,
                        transaction.Year
                    ),
                    out var existingTransaction
                )
            )
            {
                existingTransaction.Update(
                    transaction.Name,
                    new DateOnly(transaction.Year, transaction.Month, 1),
                    transaction.Value,
                    transaction.SplitType
                );
                var person = peopleMapping[transaction.PaidByPersonId];
                existingTransaction.UpdatePaidBy(person);
                existingTransaction.UpdateSharedWith([
                    .. transaction.Responsibilities.Select(r => peopleMapping[r.PersonId]),
                ]);
            }
            else
            {
                var newTransaction = new Transaction()
                {
                    Name = transaction.Name,
                    Month = new(transaction.Year, transaction.Month, 1),
                    PaidBy = peopleMapping[transaction.PaidByPersonId],
                    Value = transaction.Value,
                    SharedWith =
                    [
                        .. transaction.Responsibilities.Select(r => peopleMapping[r.PersonId]),
                    ],
                    SplitType = transaction.SplitType,
                };
                context.Add(newTransaction);
            }
        }

        await context.SaveChangesAsync(cancellationToken: cancellationToken);

        return new BackupImportResponse();
    }

    private async Task<BackupImportResponse> HandleAsync(
        Backup backup,
        CancellationToken cancellationToken = default
    )
    {
        return new BackupImportResponse();
    }
}

[JsonSerializable(typeof(BackupObject))]
internal partial class BackupSerializerContext : JsonSerializerContext;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "FormatVersion")]
[JsonDerivedType(typeof(BackupLegacy), BackupLegacy.Version)]
[JsonDerivedType(typeof(Backup), Backup.Version)]
public record BackupObject
{
    public DateTimeOffset ExportedAtUtc { get; set; }
}

public record Backup : BackupObject
{
    public const string Version = "finance-split-backup-v2";
    public required BackupPayload Data { get; set; }
}

public record BackupLegacy : BackupObject
{
    public const string Version = "finances-split-backup-v1";
    public required BackupData Data { get; set; }

    public class BackupData
    {
        public PersonLegacy[] People { get; set; } = [];
        public TransactionLegacy[] Transactions { get; set; } = [];
    }

    [JsonNamingPolicy(JsonKnownNamingPolicy.PascalCase)]
    public class PersonLegacy
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public SalaryLegacy[] Salaries { get; set; }
    }

    [JsonNamingPolicy(JsonKnownNamingPolicy.PascalCase)]
    public class SalaryLegacy
    {
        public Guid Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
    }

    [JsonNamingPolicy(JsonKnownNamingPolicy.PascalCase)]
    public class TransactionLegacy
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter<SplitType>))]
        public SplitType SplitType { get; set; }
        public Guid PaidByPersonId { get; set; }
        public bool IsRecurring { get; set; }
        public ResponsibilityLegacy[] Responsibilities { get; set; }
    }

    [JsonNamingPolicy(JsonKnownNamingPolicy.PascalCase)]
    public class ResponsibilityLegacy
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
    }
}
