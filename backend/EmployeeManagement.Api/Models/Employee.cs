namespace EmployeeManagement.Api.Models;

public enum EmployeeRole
{
    Employee = 0,
    Leader = 1,
    Director = 2
}

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string DocNumber { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public EmployeeRole Role { get; set; }
    public string PasswordHash { get; set; } = null!;

    public int? ManagerId { get; set; }
    public Employee? Manager { get; set; }
    public List<Employee> Subordinates { get; set; } = new();

    public List<EmployeePhone> Phones { get; set; } = new();

    public string FullName => $"{FirstName} {LastName}";
}
