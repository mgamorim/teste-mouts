namespace EmployeeManagement.Api.Models;

public class EmployeePhone
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; } = null!;

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}
