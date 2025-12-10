using EmployeeManagement.Api.Models;

namespace EmployeeManagement.Api.Repositories;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByEmailAsync(string email);
    Task<Employee?> GetByDocNumberAsync(string docNumber);
    Task<bool> AnyAsync();
    Task AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(Employee employee);
    Task SaveChangesAsync();
}
