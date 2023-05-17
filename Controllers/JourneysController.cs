using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BikeappAPI.Models;
using CsvHelper;
using System.Globalization;
using System.Data;
using CsvHelper.Configuration;
using BikeappAPI.Repositories;

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
        [RequestSizeLimit(100000000)]
        public async Task<IActionResult> PostJourneys(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Read the CSV file
            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                };

                var csvReader = new CsvReader(streamReader, csvConfig);

                // Read the header record
                csvReader.Read();
                csvReader.ReadHeader();

                var headerRecords = csvReader.HeaderRecord;

                if (headerRecords != null)
                {
                    // Check if all required columns are present
                    var requiredColumns = new string[] { "Departure", "Return", "Departure station id", "Departure station name", "Return station id", "Return station name", "Covered distance (m)", "Duration (sec.)" };
                    var missingColumns = requiredColumns.Where(column => !headerRecords.Contains(column)).ToList();

                    if (missingColumns.Any())
                    {
                        return BadRequest($"Missing columns in CSV file: {string.Join(", ", missingColumns)}");
                    }

                    // Create a list to hold the Journey objects
                    var journeys = new List<Journey>();

                    // Read the CSV file and convert each row to a Journey object
                    while (csvReader.Read())
                    {
                        var journey = new Journey
                        {
                            JourneyId = Guid.NewGuid(), // Generate a unique JourneyId
                            DepartureDate = csvReader.GetField<DateTime>("Departure"),
                            ReturnDate = csvReader.GetField<DateTime>("Return"),
                            DepartureStationId = csvReader.GetField<int>("Departure station id"),
                            DepartureStationName = csvReader.GetField<string>("Departure station name"),
                            ReturnStationId = csvReader.GetField<int>("Return station id"),
                            ReturnStationName = csvReader.GetField<string>("Return station name"),
                            Distance = csvReader.GetField<decimal>("Covered distance (m)"),
                            Duration = csvReader.GetField<int>("Duration (sec.)")
                        };

                        journeys.Add(journey);
                    }

                    // Pass the list of journeys to the repository method for database insertion
                    await journeysRepository.UploadJourneysFromCsv(journeys);

                    return Ok("Journeys uploaded successfully.");

                }
                else
                {
                    return BadRequest();



            }


            }
        }

        // DELETE: api/Journeys/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJourney(int id)
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
