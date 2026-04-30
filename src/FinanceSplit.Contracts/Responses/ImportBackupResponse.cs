namespace FinanceSplit.Contracts.Responses;

public record ImportBackupResponse(int PeopleImported, int SalaryRecordsImported, int TransactionsImported);
