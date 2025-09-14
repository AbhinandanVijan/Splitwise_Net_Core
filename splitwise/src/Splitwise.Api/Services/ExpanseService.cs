using Microsoft.EntityFrameworkCore;
using Splitwise.Api.Data;
using Splitwise.Api.DTOs;
using Splitwise.Api.Entities;
using Splitwise.Api.Entities.Enums;
using Splitwise.Api.Services.Interfaces;

namespace Splitwise.Api.Services;

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _db;
    public ExpenseService(AppDbContext db) => _db = db;

    public async Task<ExpenseResponse> AddExpenseAsync(AddExpenseRequest req, CancellationToken ct = default)
    {
        var group = await _db.Groups.Include(g => g.Memberships)
            .FirstOrDefaultAsync(g => g.Id == req.GroupId, ct)
            ?? throw new KeyNotFoundException("Group not found");

        var memberIds = group.Memberships.Select(m => m.UserId).ToHashSet();
        if (!memberIds.Contains(req.PayerId)) throw new InvalidOperationException("Payer not in group");
        if (!req.ParticipantIds.All(memberIds.Contains)) throw new InvalidOperationException("Participant not in group");
        if (req.Amount <= 0) throw new ArgumentException("Amount must be > 0");

        var shares = ComputeShares(req);

        var exec = _db.Database.CreateExecutionStrategy();
        return await exec.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);

            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                GroupId = req.GroupId,
                PayerId = req.PayerId,
                Amount = req.Amount,
                Description = req.Description,
                SplitMethod = (SplitMethod)req.SplitMethod,
                CreatedAt = DateTime.UtcNow
            };
            _db.Expenses.Add(expense);

            foreach (var (uid, amt) in shares)
                _db.ExpenseShares.Add(new ExpenseShare { ExpenseId = expense.Id, UserId = uid, AmountOwed = amt });

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return new ExpenseResponse(expense.Id, expense.Amount, expense.Description,
                (SplitMethodDto)expense.SplitMethod, expense.CreatedAt);
        });
    }

    private static List<(Guid userId, decimal amount)> ComputeShares(AddExpenseRequest req)
    {
        var p = req.ParticipantIds;
        var total = req.Amount;
        var list = new List<(Guid, decimal)>();

        switch (req.SplitMethod)
        {
            case SplitMethodDto.Equal:
                var even = Math.Round(total / p.Count, 2, MidpointRounding.AwayFromZero);
                for (int i = 0; i < p.Count; i++) list.Add((p[i], even));
                var diff = total - list.Sum(x => x.Item2);
                if (diff != 0) list[^1] = (list[^1].Item1, list[^1].Item2 + diff);
                break;

            case SplitMethodDto.Percent:
                if (req.Percents == null || req.Percents.Count != p.Count) throw new ArgumentException("Percents mismatch");
                if (req.Percents.Sum() != 100m) throw new ArgumentException("Percents must sum to 100");
                for (int i = 0; i < p.Count; i++)
                {
                    var share = Math.Round(total * req.Percents[i] / 100m, 2, MidpointRounding.AwayFromZero);
                    list.Add((p[i], share));
                }
                var pdiff = total - list.Sum(x => x.Item2);
                if (pdiff != 0) list[^1] = (list[^1].Item1, list[^1].Item2 + pdiff);
                break;

            case SplitMethodDto.Exact:
                if (req.ExactAmounts == null || req.ExactAmounts.Count != p.Count) throw new ArgumentException("Exacts mismatch");
                if (req.ExactAmounts.Sum() != total) throw new ArgumentException("Exacts must sum to total");
                for (int i = 0; i < p.Count; i++)
                    list.Add((p[i], Math.Round(req.ExactAmounts[i], 2, MidpointRounding.AwayFromZero)));
                break;

            default: throw new NotSupportedException();
        }
        return list;
    }
}
