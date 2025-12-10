using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Api.Models;

namespace EmployeeManagement.Api.Dtos;

public record EmployeePhoneDto(
    [property: Required, MaxLength(30)] string PhoneNumber
);

public class EmployeeCreateDto
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = null!;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = null!;

    [Required, MaxLength(50)]
    public string DocNumber { get; set; } = null!;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public EmployeeRole Role { get; set; }

    public int? ManagerId { get; set; }

    [Required]
    public List<EmployeePhoneDto> Phones { get; set; } = new();

    [Required, MinLength(6)]
    public string Password { get; set; } = null!;
}

public class EmployeeUpdateDto
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = null!;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = null!;

    [Required, MaxLength(50)]
    public string DocNumber { get; set; } = null!;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public EmployeeRole Role { get; set; }

    public int? ManagerId { get; set; }

    [Required]
    public List<EmployeePhoneDto> Phones { get; set; } = new();
}

public record EmployeeResponseDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string DocNumber,
    DateTime DateOfBirth,
    EmployeeRole Role,
    int? ManagerId,
    string? ManagerName,
    List<string> Phones
);
