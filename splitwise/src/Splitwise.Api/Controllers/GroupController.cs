using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Splitwise.Api.Data;
using Splitwise.Api.DTOs;
using Splitwise.Api.Entities;
using Splitwise.Api.Services.Interfaces;
using System.Security.Claims;

namespace Splitwise.Api.Controllers;

[ApiController]
[Route("api/groups")]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groups;
    private readonly AppDbContext _db;

    public GroupsController(IGroupService groups, AppDbContext db) { _groups = groups; _db = db; }

    [HttpPost]
    public async Task<GroupResponse> Create([FromBody] CreateGroupRequest req)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await _groups.CreateGroupAsync(req, userId);
    }

    [HttpPost("{groupId:guid}/members")]
    public async Task<IActionResult> AddMember(Guid groupId, [FromBody] AddMemberRequest req)
    { await _groups.AddMemberAsync(groupId, req.UserId); return NoContent(); }

    [HttpGet("{groupId:guid}")]
    public async Task<ActionResult<Group>> Get(Guid groupId)
        => await _db.Groups.Include(g => g.Memberships).ThenInclude(m => m.User).FirstOrDefaultAsync(g => g.Id == groupId) is { } g
            ? Ok(g) : NotFound();
}
