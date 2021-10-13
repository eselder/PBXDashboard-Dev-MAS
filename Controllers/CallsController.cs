using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBXDashboard_Dev.Models;

namespace PBXDashboard_Dev.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class CallsController : ControllerBase
    {
        private readonly DataContext _context;

        public CallsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Calls
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Call>>> GetCalls()
        {
            Console.WriteLine("Get Calls" + DateTime.Now);
            List<Call> calls =  await _context.Calls.Include(c => c.Events).ToListAsync();



            calls.ForEach(c => {

                if (c.Events != null) {

                    c.Events.ForEach(e => e.Call = null);

                }

            });

            return calls;
        }

        // GET: api/Calls/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Call>> GetCall(long id)
        {
            Call result = await _context.Calls.Include(c => c.Events).FirstOrDefaultAsync(c => c.CallID == id);

            if (result != null) {
                if (result.Events != null)
                {
                    foreach (Event e in result.Events) 
                    {
                        e.Call = null;
                    }
                }
            }
            return result;
        }

        // PUT: api/Calls/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCall(long id, Call call)
        {
            if (id != call.CallID)
            {
                return BadRequest();
            }

            _context.Entry(call).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CallExists(id))
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

        // POST: api/Calls
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Call>> PostCall(Call call)
        {
            _context.Calls.Add(call);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCall", new { id = call.CallID }, call);
        }

        // DELETE: api/Calls/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Call>> DeleteCall(long id)
        {
            var call = await _context.Calls.FindAsync(id);
            if (call == null)
            {
                return NotFound();
            }

            _context.Calls.Remove(call);
            await _context.SaveChangesAsync();

            return call;
        }

        private bool CallExists(long id)
        {
            return _context.Calls.Any(e => e.CallID == id);
        }
    }
}
