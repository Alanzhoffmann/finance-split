using FinanceSplit.Contracts.Enums;

namespace FinanceSplit.Contracts.Responses;

/// <summary>
/// TerminationType tells the UI which option to pre-select (never / count / date).
/// EndMonth and OccurrenceCount are derived convenience fields.
/// </summary>
public record RecurrenceResponse(DateOnly StartMonth, RecurrenceTerminationType TerminationType, DateOnly? EndMonth, int? OccurrenceCount);
