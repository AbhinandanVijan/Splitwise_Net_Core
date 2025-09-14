using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Splitwise.Api.DTOs;
using Splitwise.Api.Services.Interfaces;

namespace Splitwise.Api.Controllers;

[ApiController]
[Route("api/groups/{groupId:guid}/expenses")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _svc;
    public ExpensesController(IExpenseService svc) => _svc = svc;

    [HttpPost]
    public Task<ExpenseResponse> Add(Guid groupId, [FromBody] AddExpenseRequest req)
    {
        if (groupId != req.GroupId) throw new ArgumentException("Group mismatch");
        return _svc.AddExpenseAsync(req);
    }
}
