using Ceramix.Domain.Entities;
using Ceramix.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ceramix.Application.Services;

public class StudentService
{
    private readonly CeramixDbContext _context;

    public StudentService(CeramixDbContext context)
    {
        _context = context;
    }

    public async Task<List<Student>> GetAll()
    {
        return await _context.Students
            .Include(s => s.Pieces)
            .ToListAsync();
    }

    public async Task<Student?> GetById(Guid id)
    {
        return await _context.Students
            .Include(s => s.Pieces)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student> Create(string name, string email)
    {
        var student = new Student(name, email);
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<bool> Delete(Guid id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return false;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }
}