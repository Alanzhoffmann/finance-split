using System.Text.Json.Serialization;

namespace FinanceSplit.Contracts.People.Commands;

public record UpdatePersonCommand([property: JsonIgnore] Guid Id, string Name);
