using Microsoft.AspNetCore.Mvc;
using Splitwise.Api.DTOs;
using Splitwise.Api.Services.Interfaces;

namespace Splitwise.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _auth;
    public UsersController(IAuthService auth) => _auth = auth;

    [HttpPost("register")]
    public Task<AuthResponse> Register([FromBody] RegisterRequest req) => _auth.RegisterAsync(req);

    [HttpPost("login")]
    public Task<AuthResponse> Login([FromBody] LoginRequest req) => _auth.LoginAsync(req);
}
