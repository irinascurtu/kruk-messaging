using EmployeeManagement.Data;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee.Controllers
{
    [Route("api/employees/{employeeId}/[controller]")]
    [ApiController]
    public class RewardsController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public RewardsController(EmployeeDbContext context)
        {
            _context = context;
        }

        // GET: api/employees/{employeeId}/Rewards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reward>>> GetRewards(int employeeId)
        {
            return await _context.Rewards
                .Where(r => r.EmployeeId == employeeId)
                .Include(r => r.Employee)
                .ToListAsync();
        }

        // GET: api/employees/{employeeId}/Rewards/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Reward>> GetReward(int employeeId, int id)
        {
            var reward = await _context.Rewards
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.EmployeeId == employeeId && r.Id == id);

            if (reward == null)
            {
                return NotFound();
            }

            return reward;
        }

        // PUT: api/employees/{employeeId}/Rewards/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReward(int employeeId, int id, Reward reward)
        {
            if (id != reward.Id || employeeId != reward.EmployeeId)
            {
                return BadRequest();
            }

            _context.Entry(reward).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RewardExists(employeeId, id))
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

        // POST: api/employees/{employeeId}/Rewards
        [HttpPost]
        public async Task<ActionResult<Reward>> PostReward(int employeeId, Reward reward)
        {
            if (employeeId != reward.EmployeeId)
            {
                return BadRequest();
            }

            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            _context.Rewards.Add(reward);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReward", new { employeeId = employeeId, id = reward.Id }, reward);
        }

        // DELETE: api/employees/{employeeId}/Rewards/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReward(int employeeId, int id)
        {
            var reward = await _context.Rewards
                .FirstOrDefaultAsync(r => r.EmployeeId == employeeId && r.Id == id);

            if (reward == null)
            {
                return NotFound();
            }

            _context.Rewards.Remove(reward);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RewardExists(int employeeId, int id)
        {
            return _context.Rewards.Any(r => r.EmployeeId == employeeId && r.Id == id);
        }
    }
}
