using System;

namespace Splitwise.Api.Entities;

public class Group
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}