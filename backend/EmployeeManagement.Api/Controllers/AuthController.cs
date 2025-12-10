using System.Security.Claims;
using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Models;
using EmployeeManagement.Api.Repositories;
using EmployeeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEmployeeRepository _repository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IEmployeeRepository repository,
        ITokenService tokenService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _repository = repository;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("seed-first-director")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> SeedFirstDirector(RegisterRequestDto dto)
    {
        if (await _repository.AnyAsync())
            return BadRequest("Employees already exist. Use authenticated endpoints to create new ones.");

        var employee = await _authService.RegisterAsync(dto);
        employee.Role = EmployeeRole.Director;
        await _repository.UpdateAsync(employee);
        await _repository.SaveChangesAsync();

        var (token, expiresAt) = _tokenService.GenerateToken(employee);
        return Ok(new AuthResponseDto(token, expiresAt));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto dto)
    {
        var employee = await _authService.AuthenticateAsync(dto.Email, dto.Password);
        if (employee is null)
            return Unauthorized("Invalid credentials.");

        var (token, expiresAt) = _tokenService.GenerateToken(employee);
        _logger.LogInformation("User {Email} logged in", employee.Email);

        return Ok(new AuthResponseDto(token, expiresAt));
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<object> Me()
    {
        var name = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        return Ok(new { Name = name, Email = email, Role = role });
    }
}
