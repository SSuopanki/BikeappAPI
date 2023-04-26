using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BikeappAPI.Models;
using CsvHelper;
using System.Globalization;

namespace BikeappAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JourneysController : ControllerBase
    {
        private readonly BikeappContext _context;

        public JourneysController(BikeappContext context)
        {
            _context = context;
        }

        // GET: api/Journeys
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Journey>>> GetJourney()
        {
            if (_context.Journey == null)
            {
                return NotFound();
            }
            return await _context.Journey.ToListAsync();
        }

        // GET: api/Journeys/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Journey>> GetJourney(int id)
        {
            if (_context.Journey == null)
            {
                return NotFound();
            }
            var journey = await _context.Journey.FindAsync(id);

            if (journey == null)
            {
                return NotFound();
            }

            return journey;
        }

        // PUT: api/Journeys/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJourney(int id, Journey journey)
        {
            if (id != journey.JourneyId)
            {
                return BadRequest();
            }

            _context.Entry(journey).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JourneyExists(id))
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

        // POST: api/Journeys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Journey>> PostJourney(Journey journey)
        {
            if (_context.Journey == null)
            {
                return Problem("Entity set 'BikeappContext.Journey'  is null.");
            }
            _context.Journey.Add(journey);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (JourneyExists(journey.JourneyId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetJourney", new { id = journey.JourneyId }, journey);
        }

        // POST: api/UploadJoyrneys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CSV")]
        public async Task<IActionResult> PostJourneys([FromForm] IFormFile file)
        {
            //Read the CSV file data into a MemoryStream
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                //Read the CSV data from the MemoryStream using CsvHelper
                using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<dynamic>().ToList();

                    // TODO: Process the CSV data
                }
            }

            return Ok();
        }

        // DELETE: api/Journeys/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJourney(int id)
        {
            if (_context.Journey == null)
            {
                return NotFound();
            }
            var journey = await _context.Journey.FindAsync(id);
            if (journey == null)
            {
                return NotFound();
            }

            _context.Journey.Remove(journey);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JourneyExists(int id)
        {
            return (_context.Journey?.Any(e => e.JourneyId == id)).GetValueOrDefault();
        }
    }
}
