﻿using EmployeeManagement.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
namespace EmployeeManagement.Controllers
{

}
[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly EmployeeDbContext _context;

    public EmployeesController(EmployeeDbContext context)
    {
        _context = context;
    }

    // GET: api/Employees
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeManagement.Models.Employee>>> GetEmployees()
    {
        return await _context.Employees.ToListAsync();
    }

    // GET: api/Employees/5
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeManagement.Models.Employee>> GetEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        return employee;
    }

    // PUT: api/Employees/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEmployee(int id, EmployeeManagement.Models.Employee employee)
    {
        if (id != employee.Id)
        {
            return BadRequest();
        }

        _context.Entry(employee).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EmployeeExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Employees
    [HttpPost]
    public async Task<ActionResult<EmployeeManagement.Models.Employee>> PostEmployee(EmployeeManagement.Models.Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
    }

    // DELETE: api/Employees/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EmployeeExists(int id)
    {
        return _context.Employees.Any(e => e.Id == id);
    }
}


