using System;

namespace Splitwise.Api.Entities;

public class ExpenseShare
{
    public Guid ExpenseId { get; set; }
    public Expense Expense { get; set; } = default!;

    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public decimal AmountOwed { get; set; }
}