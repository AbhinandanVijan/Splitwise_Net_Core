using Splitwise.Api.DTOs;

namespace Splitwise.Api.Services.Interfaces;
public interface IExpenseService
{
    Task<ExpenseResponse> AddExpenseAsync(AddExpenseRequest req, CancellationToken ct = default);
}
