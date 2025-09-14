using Microsoft.EntityFrameworkCore;
using Splitwise.Api.Data;
using Splitwise.Api.DTOs;
using Splitwise.Api.Entities;
using Splitwise.Api.Services.Interfaces;

namespace Splitwise.Api.Services;

public class GroupService : IGroupService
{
    private readonly AppDbContext _db;
    public GroupService(AppDbContext db) => _db = db;

    public async Task<GroupResponse> CreateGroupAsync(CreateGroupRequest req, Guid creatorId)
    {
        var g = new Group { Id = Guid.NewGuid(), Name = req.Name, Description = req.Description };
        _db.Groups.Add(g);
        _db.Memberships.Add(new Membership { GroupId = g.Id, UserId = creatorId });
        await _db.SaveChangesAsync();
        return new GroupResponse(g.Id, g.Name);
    }

    public async Task AddMemberAsync(Guid groupId, Guid userId)
    {
        var exists = await _db.Memberships.AnyAsync(m => m.GroupId == groupId && m.UserId == userId);
        if (exists) return;
        _db.Memberships.Add(new Membership { GroupId = groupId, UserId = userId });
        await _db.SaveChangesAsync();
    }
}
