using System.Security.Cryptography;
using System.Text;
using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Models;
using EmployeeManagement.Api.Repositories;

namespace EmployeeManagement.Api.Services;

public interface IAuthService
{
    Task<Employee> RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default);
    Task<Employee?> AuthenticateAsync(string email, string password, CancellationToken ct = default);
    bool IsAdult(DateTime dateOfBirth);
}

public class AuthService : IAuthService
{
    private readonly IEmployeeRepository _repository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IEmployeeRepository repository, ILogger<AuthService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Employee> RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default)
    {
        if (!IsAdult(dto.DateOfBirth))
        {
            throw new InvalidOperationException("Employee must be at least 18 years old.");
        }

        var existing = await _repository.GetByEmailAsync(dto.Email);
        if (existing is not null)
        {
            throw new InvalidOperationException("E-mail already exists.");
        }

        var existingDoc = await _repository.GetByDocNumberAsync(dto.DocNumber);
        if (existingDoc is not null)
        {
            throw new InvalidOperationException("Doc number already exists.");
        }

        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            DocNumber = dto.DocNumber,
            DateOfBirth = dto.DateOfBirth,
            Role = EmployeeRole.Employee,
            PasswordHash = HashPassword(dto.Password, dto.DocNumber)
        };

        await _repository.AddAsync(employee);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Registered new employee {Email}", employee.Email);

        return employee;
    }

    public async Task<Employee?> AuthenticateAsync(string email, string password, CancellationToken ct = default)
    {
        var employee = await _repository.GetByEmailAsync(email);
        if (employee is null)
        {
            return null;
        }

        var hash = HashPassword(password, employee.DocNumber);
        if (employee.PasswordHash != hash)
        {
            return null;
        }

        return employee;
    }

    public bool IsAdult(DateTime dateOfBirth)
    {
        var today = DateTime.UtcNow.Date;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age >= 18;
    }

    private static string HashPassword(string password, string? saltSource = null)
    {
        var saltBytes = Encoding.UTF8.GetBytes(saltSource ?? "default-salt");
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
        var hashBytes = pbkdf2.GetBytes(32);
        return Convert.ToBase64String(hashBytes);
    }
}
