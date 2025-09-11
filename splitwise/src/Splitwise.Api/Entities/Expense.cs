using System;

namespace Splitwise.Api.Entities;
using Splitwise.Api.Entities.Enums;

public class Expense
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = default!;

    public Guid PayerId { get; set; }
    public User Payer { get; set; } = default!;

    public decimal Amount { get; set; }
    public string Description { get; set; } = default!;
    public SplitMethod SplitMethod { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ExpenseShare> Shares { get; set; } = new List<ExpenseShare>();
}