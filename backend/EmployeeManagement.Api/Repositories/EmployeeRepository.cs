using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _db;

    public EmployeeRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Employee>> GetAllAsync()
    {
        return await _db.Employees
            .Include(e => e.Manager)
            .Include(e => e.Phones)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _db.Employees
            .Include(e => e.Manager)
            .Include(e => e.Phones)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public Task<Employee?> GetByEmailAsync(string email)
    {
        return _db.Employees.FirstOrDefaultAsync(e => e.Email == email);
    }

    public Task<Employee?> GetByDocNumberAsync(string docNumber)
    {
        return _db.Employees.FirstOrDefaultAsync(e => e.DocNumber == docNumber);
    }

    public async Task<bool> AnyAsync()
    {
        return await _db.Employees.AnyAsync();
    }

    public async Task AddAsync(Employee employee)
    {
        await _db.Employees.AddAsync(employee);
    }

    public Task UpdateAsync(Employee employee)
    {
        _db.Employees.Update(employee);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Employee employee)
    {
        _db.Employees.Remove(employee);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
