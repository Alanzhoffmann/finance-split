using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Requests;
using FinanceSplit.Contracts.Responses;
using FinanceSplit.Data;
using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FinanceSplit.Application.Services;

public class ImportService(FinanceSplitDbContext db)
{
    private const string SupportedFormatVersion = "finances-split-backup-v1";

    public async Task<ImportBackupResponse> ImportAsync(ImportBackupRequest request, CancellationToken ct = default)
    {
        if (request.FormatVersion != SupportedFormatVersion)
        {
            throw new ArgumentException($"Unsupported format version '{request.FormatVersion}'. Expected '{SupportedFormatVersion}'.");
        }

        var personMap = await ImportPeopleAsync(request.Data.People, ct);
        var transactionsImported = await ImportTransactionsAsync(request.Data.Transactions, personMap, ct);

        await db.SaveChangesAsync(ct);

        return new ImportBackupResponse(personMap.Count, request.Data.People.Sum(p => p.Salaries.Count), transactionsImported);
    }

    private async Task<Dictionary<Guid, Person>> ImportPeopleAsync(List<ImportPerson> importPeople, CancellationToken ct)
    {
        var existingPeople = await db.People.ToListAsync(ct);
        var personMap = new Dictionary<Guid, Person>();

        foreach (var importPerson in importPeople)
        {
            var existing = existingPeople.FirstOrDefault(p => p.Name.Equals(importPerson.Name, StringComparison.OrdinalIgnoreCase));

            if (existing is not null)
            {
                personMap[importPerson.Id] = existing;
                ImportSalaries(existing, importPerson.Salaries);
            }
            else
            {
                var person = new Person(importPerson.Name);
                ImportSalaries(person, importPerson.Salaries);
                await db.People.AddAsync(person, ct);
                personMap[importPerson.Id] = person;
            }
        }

        return personMap;
    }

    private static void ImportSalaries(Person person, List<ImportSalaryRecord> salaries)
    {
        foreach (var importSalary in salaries)
        {
            var date = new DateOnly(importSalary.Year, importSalary.Month, 1);
            var existingSalary = person.Salaries.FirstOrDefault(s => s.Date == date);
            if (existingSalary is null)
            {
                person.AddSalary(Salary.ForMonth(importSalary.Year, importSalary.Month, importSalary.Amount));
            }
        }
    }

    private async Task<int> ImportTransactionsAsync(List<ImportTransaction> importTransactions, Dictionary<Guid, Person> personMap, CancellationToken ct)
    {
        var count = 0;

        foreach (var importTx in importTransactions)
        {
            if (!personMap.TryGetValue(importTx.PaidByPersonId, out var paidBy))
            {
                continue;
            }

            // Transactions are paid from the previous month's salary, so place them in the next month
            var originalDate = new DateTime(importTx.Year, importTx.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var date = originalDate.AddMonths(1);

            var exists = await db.Transactions.AnyAsync(t => t.Title == importTx.Name && t.Amount == importTx.Value && t.Date == date, ct);

            if (exists)
            {
                continue;
            }

            var participants = importTx
                .Responsibilities.Select(r => personMap.TryGetValue(r.PersonId, out var p) ? p : null)
                .Where(p => p is not null)
                .Cast<Person>()
                .ToList();

            var splitType = MapSplitType(importTx.SplitType);
            var splitPay = splitType switch
            {
                SplitType.None => SplitPay.CreateNoneSplit(paidBy),
                SplitType.Even => SplitPay.CreateEvenSplit(participants),
                SplitType.Ratio => SplitPay.CreateRatioSplit(participants),
                _ => SplitPay.CreateEvenSplit(participants),
            };

            Recurrence? recurrence = importTx.IsRecurring ? Recurrence.Forever(DateOnly.FromDateTime(date)) : null;

            var transaction = new Transaction(importTx.Name, importTx.Value, paidBy, splitPay, date, recurrence);

            await db.Transactions.AddAsync(transaction, ct);
            count++;
        }

        return count;
    }

    private static SplitType MapSplitType(SplitType importSplitType) => importSplitType;
}
