using System.Text.Json.Serialization;
using FinanceSplit.Contracts.Goals.Commands;
using FinanceSplit.Contracts.Goals.DTOs;
using FinanceSplit.Contracts.Goals.Services;

namespace FinanceSplit.Client.Clients;

internal partial class GoalService(HttpClient client) : IGoalService
{
    public async Task<IReadOnlyList<GoalDto>?> GetGoalsAsync(
        CancellationToken cancellationToken = default
    ) =>
        await client.GetFromJsonAsync(
            "api/goals",
            GoalSerializerContext.Default.IReadOnlyListGoalDto,
            cancellationToken
        );

    public async Task<GoalDto?> CreateGoalAsync(
        CreateGoalCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.PostAsJsonAsync(
            $"api/goals",
            command,
            GoalSerializerContext.Default.CreateGoalCommand,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(
            GoalSerializerContext.Default.GoalDto,
            cancellationToken
        );
    }

    public async Task<GoalDto?> UpdateGoalAsync(
        UpdateGoalCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.PutAsJsonAsync(
            $"api/goals/{command.Id}",
            command,
            GoalSerializerContext.Default.UpdateGoalCommand,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(
            GoalSerializerContext.Default.GoalDto,
            cancellationToken
        );
    }

    public async Task DeleteGoalAsync(
        DeleteGoalCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.DeleteAsync($"api/goals/{command.Id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(IReadOnlyList<GoalDto>))]
    [JsonSerializable(typeof(CreateGoalCommand))]
    [JsonSerializable(typeof(UpdateGoalCommand))]
    partial class GoalSerializerContext : JsonSerializerContext;
}
