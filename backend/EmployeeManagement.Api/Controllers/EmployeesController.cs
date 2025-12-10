using System.Security.Claims;
using System.Security.Cryptography;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Mapping;
using EmployeeManagement.Api.Models;
using EmployeeManagement.Api.Repositories;
using EmployeeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _repository;
    private readonly IAuthService _authService;
    private readonly ILogger<EmployeesController> _logger;
    private readonly AppDbContext _db;

    public EmployeesController(
        IEmployeeRepository repository,
        IAuthService authService,
        ILogger<EmployeesController> logger,
        AppDbContext db)
    {
        _repository = repository;
        _authService = authService;
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetAll()
    {
        var employees = await _repository.GetAllAsync();
        return Ok(employees.Select(e => e.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeResponseDto>> GetById(int id)
    {
        var employee = await _repository.GetByIdAsync(id);
        if (employee is null) return NotFound();
        return Ok(employee.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeResponseDto>> Create(EmployeeCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (!_authService.IsAdult(dto.DateOfBirth))
            return BadRequest("Employee must be at least 18 years old.");

        if (dto.Phones is null || dto.Phones.Count == 0)
            return BadRequest("At least one phone is required.");

        var currentUserRole = GetCurrentUserRole();
        if (!CanCreateRole(currentUserRole, dto.Role))
            return Forbid("You cannot create a user with higher permissions than yours.");

        if (await _repository.GetByEmailAsync(dto.Email) is not null)
            return BadRequest("E-mail already exists.");

        if (await _repository.GetByDocNumberAsync(dto.DocNumber) is not null)
            return BadRequest("Doc number already exists.");

        if (dto.ManagerId.HasValue)
        {
            var manager = await _repository.GetByIdAsync(dto.ManagerId.Value);
            if (manager is null)
                return BadRequest("Invalid manager.");
        }

        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            DocNumber = dto.DocNumber,
            DateOfBirth = dto.DateOfBirth,
            Role = dto.Role,
            ManagerId = dto.ManagerId,
            PasswordHash = "",
            Phones = dto.Phones.Select(p => new EmployeePhone { PhoneNumber = p.PhoneNumber }).ToList()
        };

        employee.PasswordHash = AuthServiceHash(dto.Password, employee.DocNumber);

        await _repository.AddAsync(employee);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Employee {Id} created by {User}", employee.Id, User.Identity?.Name);

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EmployeeResponseDto>> Update(int id, EmployeeUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var employee = await _repository.GetByIdAsync(id);
        if (employee is null) return NotFound();

        var currentUserRole = GetCurrentUserRole();
        if (!CanCreateRole(currentUserRole, dto.Role))
            return Forbid("You cannot promote a user to a higher permission than yours.");

        if (!_authService.IsAdult(dto.DateOfBirth))
            return BadRequest("Employee must be at least 18 years old.");

        if (dto.Phones is null || dto.Phones.Count == 0)
            return BadRequest("At least one phone is required.");

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Email = dto.Email;
        employee.DocNumber = dto.DocNumber;
        employee.DateOfBirth = dto.DateOfBirth;
        employee.Role = dto.Role;
        employee.ManagerId = dto.ManagerId;

        await _db.Entry(employee).Collection(e => e.Phones).LoadAsync();
        _db.EmployeePhones.RemoveRange(employee.Phones);
        employee.Phones = dto.Phones
            .Select(p => new EmployeePhone { PhoneNumber = p.PhoneNumber, EmployeeId = employee.Id })
            .ToList();

        await _repository.UpdateAsync(employee);
        await _repository.SaveChangesAsync();

        return Ok(employee.ToDto());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _repository.GetByIdAsync(id);
        if (employee is null) return NotFound();

        await _repository.DeleteAsync(employee);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    private EmployeeRole GetCurrentUserRole()
    {
        var roleString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        return Enum.TryParse<EmployeeRole>(roleString, out var role)
            ? role
            : EmployeeRole.Employee;
    }

    private static bool CanCreateRole(EmployeeRole currentUserRole, EmployeeRole newUserRole)
        => newUserRole <= currentUserRole;

    private static string AuthServiceHash(string password, string docNumber)
    {
        var saltBytes = System.Text.Encoding.UTF8.GetBytes(docNumber);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
        var hashBytes = pbkdf2.GetBytes(32);
        return Convert.ToBase64String(hashBytes);
    }
}
