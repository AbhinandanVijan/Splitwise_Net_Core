using System;

namespace Splitwise.Api.DTOs;


public enum SplitMethodDto { Equal = 0, Percent = 1, Exact = 2 }

public record AddExpenseRequest(
    Guid GroupId,
    Guid PayerId,
    decimal Amount,
    string Description,
    SplitMethodDto SplitMethod,
    List<Guid> ParticipantIds,
    List<decimal>? Percents,
    List<decimal>? ExactAmounts
);

public record ExpenseResponse(
    Guid ExpenseId,
    decimal Amount,
    string Description,
    SplitMethodDto SplitMethod,
    DateTime CreatedAt
);

public record BalanceLine(Guid CounterpartyId, decimal YouOwe, decimal OweYou);
