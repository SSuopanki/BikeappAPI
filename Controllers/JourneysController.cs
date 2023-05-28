using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BikeappAPI.Models;
using CsvHelper;
using System.Globalization;
using System.Data;
using CsvHelper.Configuration;
using BikeappAPI.Repositories;
using Microsoft.AspNetCore.Http;

namespace BikeappAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JourneysController : ControllerBase
    {
        private readonly JourneysRepository journeysRepository;
        private readonly BikeappContext context;

        public JourneysController(BikeappContext context, JourneysRepository journeysRepository)
        {
            this.journeysRepository = journeysRepository;
            this.context = context;
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
        public async Task<ActionResult<Journey>> GetJourney(Guid id)
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
        public async Task<IActionResult> PutJourney(Guid id, Journey journey)
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
        [RequestSizeLimit(10000000)]
        public async Task<IActionResult> PostJourneys(IFormFile formFile)
        {
            await journeysRepository.UploadJourneysFromCsv(formFile);

            return Ok("File uploaded succesfully");
        }

        // DELETE: api/Journeys/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJourney(Guid id)
        {
            if (context.Journey == null)
            {
                return NotFound();
            }
            var journey = await context.Journey.FindAsync(id);
            if (journey == null)
            {
                return NotFound();
            }

            context.Journey.Remove(journey);
            await context.SaveChangesAsync();

            return NoContent();
        }



        private bool JourneyExists(Guid id)
        {
            return (context.Journey?.Any(e => e.JourneyId == id)).GetValueOrDefault();
        }
    }
}
