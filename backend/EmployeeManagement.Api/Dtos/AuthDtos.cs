using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Dtos;

public class RegisterRequestDto
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

    [Required, MinLength(6)]
    public string Password { get; set; } = null!;
}

public class LoginRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}

public record AuthResponseDto(
    string Token,
    DateTime ExpiresAt
);
