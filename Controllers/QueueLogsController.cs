using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBXDashboard_Dev.Models;

namespace PBXDashboard_Dev.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueueLogsController : ControllerBase
    {
        private readonly DataContext _context;

        public QueueLogsController(DataContext context)
        {
            _context = context;
        }

        
        // GET: api/QueueLogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QueueLog>>> GetQueueLogs()
        {
            return await _context.QueueLogs.ToListAsync();
        }

        // GET: api/QueueLogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QueueLog>> GetQueueLog(long id)
        {
            var queueLog = await _context.QueueLogs.FindAsync(id);

            if (queueLog == null)
            {
                return NotFound();
            }

            return queueLog;
        }

        // PUT: api/QueueLogs/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQueueLog(long id, QueueLog queueLog)
        {
            if (id != queueLog.ID)
            {
                return BadRequest();
            }

            _context.Entry(queueLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QueueLogExists(id))
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

        // POST: api/QueueLogs
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<QueueLog>> PostQueueLog(QueueLog queueLog)
        {
            _context.QueueLogs.Add(queueLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQueueLog", new { id = queueLog.ID }, queueLog);
        }

        // DELETE: api/QueueLogs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<QueueLog>> DeleteQueueLog(long id)
        {
            var queueLog = await _context.QueueLogs.FindAsync(id);
            if (queueLog == null)
            {
                return NotFound();
            }

            _context.QueueLogs.Remove(queueLog);
            await _context.SaveChangesAsync();

            return queueLog;
        }

        private bool QueueLogExists(long id)
        {
            return _context.QueueLogs.Any(e => e.ID == id);
        }
    }
}
