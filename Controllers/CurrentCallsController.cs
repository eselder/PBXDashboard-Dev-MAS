using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PBXDashboard_Dev.Models;

namespace PBXDashboard_Dev.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrentCallsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMemoryCache _cache;

        public CurrentCallsController(IMemoryCache cache)
        {
            _cache = cache;
        }

        // GET: api/CurrentCalls
        [HttpGet]
        public string GetCurrentCalls()
        {
            var calls = "";
            _cache.TryGetValue("CurrentCalls", out calls);
            return calls;
            // return await _context.CurrentCalls.ToListAsync();
        }

        // GET: api/CurrentCalls/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CurrentCall>> GetCurrentCall(long id)
        {
            var currentCall = await _context.CurrentCalls.FindAsync(id);

            if (currentCall == null)
            {
                return NotFound();
            }

            return currentCall;
        }

        // PUT: api/CurrentCalls/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCurrentCall(long id, CurrentCall currentCall)
        {
            if (id != currentCall.ID)
            {
                return BadRequest();
            }

            _context.Entry(currentCall).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrentCallExists(id))
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

        // POST: api/CurrentCalls
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<CurrentCall>> PostCurrentCall(CurrentCall currentCall)
        {
            _context.CurrentCalls.Add(currentCall);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCurrentCall", new { id = currentCall.ID }, currentCall);
        }

        // DELETE: api/CurrentCalls/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CurrentCall>> DeleteCurrentCall(long id)
        {
            var currentCall = await _context.CurrentCalls.FindAsync(id);
            if (currentCall == null)
            {
                return NotFound();
            }

            _context.CurrentCalls.Remove(currentCall);
            await _context.SaveChangesAsync();

            return currentCall;
        }

        private bool CurrentCallExists(long id)
        {
            return _context.CurrentCalls.Any(e => e.ID == id);
        }
    }
}
