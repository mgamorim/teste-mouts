using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeManagement.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Api.Services;

public interface ITokenService
{
    (string token, DateTime expiresAt) GenerateToken(Employee employee);
}

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public TokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public (string token, DateTime expiresAt) GenerateToken(Employee employee)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(_settings.ExpiresInMinutes);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, employee.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, employee.Email),
            new Claim(ClaimTypes.Name, employee.FullName),
            new Claim(ClaimTypes.Role, employee.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenString, expires);
    }
}
