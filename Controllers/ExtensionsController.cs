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
    public class ExtensionsController : ControllerBase
    {
        private readonly DataContext _context;

        public ExtensionsController(DataContext context)
        {
            _context = context;
        }

        
        // GET: api/Extensions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Extension>>> GetExtensions()
        {
            Console.WriteLine("Get Extensions " + DateTime.Now);
            return await _context.Extensions.ToListAsync();
        }

        // GET: api/Extensions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Extension>> GetExtension(long id)
        {
            var extension = await _context.Extensions.FindAsync(id);

            if (extension == null)
            {
                return NotFound();
            }

            return extension;
        }

        // PUT: api/Extensions/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExtension(long id, Extension extension)
        {
            if (id != extension.Id)
            {
                return BadRequest();
            }

            _context.Entry(extension).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExtensionExists(id))
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

        // POST: api/Extensions
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Extension>> PostExtension(Extension extension)
        {
            _context.Extensions.Add(extension);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExtension", new { id = extension.Id }, extension);
        }

        // DELETE: api/Extensions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Extension>> DeleteExtension(long id)
        {
            var extension = await _context.Extensions.FindAsync(id);
            if (extension == null)
            {
                return NotFound();
            }

            _context.Extensions.Remove(extension);
            await _context.SaveChangesAsync();

            return extension;
        }

        private bool ExtensionExists(long id)
        {
            return _context.Extensions.Any(e => e.Id == id);
        }
    }
}
