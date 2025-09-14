using Microsoft.EntityFrameworkCore;
using Splitwise.Api.Data;
using Splitwise.Api.Entities;
using Splitwise.Api.Services.Interfaces;

namespace Splitwise.Api.Services;

public class SettlementService : ISettlementService
{
    private readonly AppDbContext _db;
    public SettlementService(AppDbContext db) => _db = db;

    public async Task<List<Transaction>> GenerateGroupSettlementsAsync(Guid groupId, CancellationToken ct = default)
    {
        var group = await _db.Groups.Include(g => g.Memberships).FirstOrDefaultAsync(g => g.Id == groupId, ct)
                    ?? throw new KeyNotFoundException();
        var members = group.Memberships.Select(m => m.UserId).ToList();

        var paid = await _db.Expenses.Where(e => e.GroupId == groupId)
            .GroupBy(e => e.PayerId).Select(g => new { UserId = g.Key, Paid = g.Sum(e => e.Amount) })
            .ToDictionaryAsync(x => x.UserId, x => x.Paid, ct);

        var expenseIds = await _db.Expenses.Where(e => e.GroupId == groupId).Select(e => e.Id).ToListAsync(ct);
        var owed = await _db.ExpenseShares.Where(s => expenseIds.Contains(s.ExpenseId))
            .GroupBy(s => s.UserId).Select(g => new { UserId = g.Key, Owed = g.Sum(s => s.AmountOwed) })
            .ToDictionaryAsync(x => x.UserId, x => x.Owed, ct);

        var net = new Dictionary<Guid, decimal>();
        foreach (var u in members)
        {
            paid.TryGetValue(u, out var p); owed.TryGetValue(u, out var o);
            net[u] = Math.Round((p - o), 2, MidpointRounding.AwayFromZero);
        }

        var debtors = new List<(Guid u, decimal amt)>();
        var creditors = new List<(Guid u, decimal amt)>();
        foreach (var kv in net)
        {
            if (kv.Value < 0) debtors.Add((kv.Key, -kv.Value));
            else if (kv.Value > 0) creditors.Add((kv.Key, kv.Value));
        }

        debtors.Sort((a, b) => b.amt.CompareTo(a.amt));
        creditors.Sort((a, b) => b.amt.CompareTo(a.amt));

        var txs = new List<Transaction>();
        int i = 0, j = 0;
        while (i < debtors.Count && j < creditors.Count)
        {
            var d = debtors[i]; var c = creditors[j];
            var pay = Math.Min(d.amt, c.amt);
            txs.Add(new Transaction
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                SenderId = d.u,
                ReceiverId = c.u,
                Amount = pay,
                Notes = "Auto-settlement",
                CreatedAt = DateTime.UtcNow
            });
            d.amt -= pay; c.amt -= pay; debtors[i] = d; creditors[j] = c;
            if (d.amt == 0) i++; if (c.amt == 0) j++;
        }

        var exec = _db.Database.CreateExecutionStrategy();
        await exec.ExecuteAsync(async () =>
        {
            await using var dbtx = await _db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);
            _db.Transactions.AddRange(txs);
            await _db.SaveChangesAsync(ct);
            await dbtx.CommitAsync(ct);
        });

        return txs;
    }
}
