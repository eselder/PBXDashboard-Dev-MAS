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
    public class ExtensionStatusesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMemoryCache _cache;

        public ExtensionStatusesController(IMemoryCache cache)
        {
            _cache = cache;
        }

        // GET: api/ExtensionStatuses
        [HttpGet]
        public string GetExtensionStatuses()
        {
            var statuses = "";
            _cache.TryGetValue("ExtensionStatuses", out statuses);
            return statuses;
            // return await _context.ExtensionStatuses.ToListAsync();
        }

        // GET: api/ExtensionStatuses/5

        // PUT: api/ExtensionStatuses/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.

    }
}
