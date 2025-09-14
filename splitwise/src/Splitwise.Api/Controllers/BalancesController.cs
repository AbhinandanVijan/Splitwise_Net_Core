using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Splitwise.Api.Data;
using Splitwise.Api.DTOs;

namespace Splitwise.Api.Controllers;

[ApiController]
[Route("api/groups/{groupId:guid}/balances")]
[Authorize]
public class BalancesController : ControllerBase
{
    private readonly AppDbContext _db; public BalancesController(AppDbContext db) => _db = db;

    [HttpGet("users/{userId:guid}")]
    public async Task<IEnumerable<BalanceLine>> GetUserBalances(Guid groupId, Guid userId)
    {
        var paidByUser = await _db.Expenses.Where(e => e.GroupId == groupId && e.PayerId == userId)
            .SelectMany(e => e.Shares)
            .GroupBy(s => s.UserId)
            .Select(g => new { Other = g.Key, Amount = g.Sum(s => s.AmountOwed) })
            .ToDictionaryAsync(x => x.Other, x => x.Amount);

        var userOwes = await _db.ExpenseShares
            .Where(s => _db.Expenses.Any(e => e.Id == s.ExpenseId && e.GroupId == groupId) && s.UserId == userId)
            .Join(_db.Expenses, s => s.ExpenseId, e => e.Id, (s, e) => new { e.PayerId, s.AmountOwed })
            .GroupBy(x => x.PayerId).Select(g => new { Other = g.Key, Amount = g.Sum(x => x.AmountOwed) })
            .ToDictionaryAsync(x => x.Other, x => x.Amount);

        var memberIds = await _db.Memberships.Where(m => m.GroupId == groupId).Select(m => m.UserId).ToListAsync();
        var lines = new List<BalanceLine>();
        foreach (var other in memberIds.Where(o => o != userId))
        {
            paidByUser.TryGetValue(other, out var oweYou);
            userOwes.TryGetValue(other, out var youOwe);
            lines.Add(new BalanceLine(other, youOwe, oweYou));
        }
        return lines;
    }
}
