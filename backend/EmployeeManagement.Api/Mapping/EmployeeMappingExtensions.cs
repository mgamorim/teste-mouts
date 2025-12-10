using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Api.Models;

namespace EmployeeManagement.Api.Mapping;

public static class EmployeeMappingExtensions
{
    public static EmployeeResponseDto ToDto(this Employee employee)
    {
        return new EmployeeResponseDto(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.DocNumber,
            employee.DateOfBirth,
            employee.Role,
            employee.ManagerId,
            employee.Manager?.FullName,
            employee.Phones.Select(p => p.PhoneNumber).ToList()
        );
    }
}
