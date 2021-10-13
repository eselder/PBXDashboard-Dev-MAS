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
    public class TalkTimeRecordsController : ControllerBase
    {
        private readonly DataContext _context;

        public TalkTimeRecordsController(DataContext context)
        {
            _context = context;
        }

        
        // GET: api/TalkTimeRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TalkTimeRecord>>> GetTalkTimeRecords()
        {
            Console.WriteLine("Get TalkTimeRecords " + DateTime.Now);
            return await _context.TalkTimeRecords.ToListAsync();
        }

        // GET: api/TalkTimeRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TalkTimeRecord>> GetTalkTimeRecord(long id)
        {
            var talkTimeRecord = await _context.TalkTimeRecords.FindAsync(id);

            if (talkTimeRecord == null)
            {
                return NotFound();
            }

            return talkTimeRecord;
        }

        // PUT: api/TalkTimeRecords/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTalkTimeRecord(long id, TalkTimeRecord talkTimeRecord)
        {
            if (id != talkTimeRecord.ID)
            {
                return BadRequest();
            }

            _context.Entry(talkTimeRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TalkTimeRecordExists(id))
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

        // POST: api/TalkTimeRecords
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TalkTimeRecord>> PostTalkTimeRecord(TalkTimeRecord talkTimeRecord)
        {
            _context.TalkTimeRecords.Add(talkTimeRecord);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTalkTimeRecord", new { id = talkTimeRecord.ID }, talkTimeRecord);
        }

        // DELETE: api/TalkTimeRecords/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TalkTimeRecord>> DeleteTalkTimeRecord(long id)
        {
            var talkTimeRecord = await _context.TalkTimeRecords.FindAsync(id);
            if (talkTimeRecord == null)
            {
                return NotFound();
            }

            _context.TalkTimeRecords.Remove(talkTimeRecord);
            await _context.SaveChangesAsync();

            return talkTimeRecord;
        }

        private bool TalkTimeRecordExists(long id)
        {
            return _context.TalkTimeRecords.Any(e => e.ID == id);
        }
    }
}
