using Splitwise.Api.Entities;

namespace Splitwise.Api.Services.Interfaces;
public interface ISettlementService
{
    Task<List<Transaction>> GenerateGroupSettlementsAsync(Guid groupId, CancellationToken ct = default);
}
