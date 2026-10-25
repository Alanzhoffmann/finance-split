using System.Text.Json.Serialization;

namespace FinanceSplit.Contracts.Commands;

public record UpdatePersonCommand([property: JsonIgnore] Guid Id, string Name);
