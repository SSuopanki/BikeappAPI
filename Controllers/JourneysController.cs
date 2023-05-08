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
using System.Data;
using Microsoft.Data.SqlClient;
using CsvHelper.Configuration;
using BikeappAPI.Repositories;

namespace BikeappAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JourneysController : ControllerBase
    {
        private readonly JourneysRepository journeysRepository;

        private readonly BikeappContext _context;

        public JourneysController(BikeappContext context)
        {
            journeysRepository = new JourneysRepository(context);
            _context = context;
        }

        // GET: api/Journeys
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Journey>>> GetJourney()
        {
            var journeys = await journeysRepository.GetAllJourneys();
            if (!journeys.Any())
            {
                return NotFound();
            }
            return Ok(journeys);
        }

        // GET: api/Journeys/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Journey>> GetJourney(int id)
        {
            var journey = await journeysRepository.GetJourneyById(id);
            if (journey == null)
            {
                return NotFound();
            }

            return Ok(journey);
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

            try
            {
                await journeysRepository.UpdateJourney(journey);
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
            try
            {
                await journeysRepository.CreateJourney(journey);
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
