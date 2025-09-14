using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Splitwise.Api.Entities;
using Splitwise.Api.Services.Interfaces;

namespace Splitwise.Api.Controllers;

[ApiController]
[Route("api/groups/{groupId:guid}/settlements")]
[Authorize]
public class SettlementsController : ControllerBase
{
    private readonly ISettlementService _svc;
    public SettlementsController(ISettlementService svc) => _svc = svc;

    [HttpPost("generate")]
    public Task<List<Transaction>> Generate(Guid groupId)
        => _svc.GenerateGroupSettlementsAsync(groupId);
}
