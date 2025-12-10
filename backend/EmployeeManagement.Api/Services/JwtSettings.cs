namespace EmployeeManagement.Api.Services;

public class JwtSettings
{
    public string Issuer { get; set; } = "EmployeeManagement";
    public string Audience { get; set; } = "EmployeeManagementClient";
    public string SecretKey { get; set; } = "MINHA_SUPER_CHAVE_DEV_123456789_ABC";
    public int ExpiresInMinutes { get; set; } = 60;
}
