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
using BikeappAPI.Repositories;
using BikeappAPI.Repository;

namespace BikeappAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationsController : ControllerBase
    {
        private readonly StationsRepository stationsRepository;
        private readonly BikeappContext context;

        public StationsController(BikeappContext context, StationsRepository stationsRepository)
        {
            this.stationsRepository= stationsRepository;
            this.context = context;
        }

        // GET: api/Stations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Station>>> GetStation()
        {
            var station = await stationsRepository.GetAllStations();
            if (!station.Any())
            {
                return NotFound();
            }
            return Ok(station);
        }

        // GET: api/Stations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Station>> GetStation(int id)
        {
            var station = await stationsRepository.GetStationById(id);
            if (station == null)
            {
                return NotFound();
            }

            return Ok(station);
        }

        // PUT: api/Stations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStation(int id, Station station)
        {
            if (id != station.Id)
            {
                return BadRequest();
            }

            try
            {
                await stationsRepository.UpdateStation(station);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StationExists(id))
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

        // POST: api/Stations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Station>> PostStation(Station station)
        {
            try
            {
                await stationsRepository.UpdateStation(station);
            }
            catch (DbUpdateException)
            {
                if (StationExists(  station.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetJourney", new { id = station.Id }, station);
        }

        // POST: api/UploadJoyrneys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CSV")]
        [RequestSizeLimit(10000000)]
        public async Task<IActionResult> PostStations(IFormFile formFile)
        {
            await stationsRepository.UploadStationsFromCsv(formFile);

            return Ok("File uploaded succesfully");
        }

        // DELETE: api/Stations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStation(int id)
        {
            if (context.Station == null)
            {
                return NotFound();
            }
            var station = await context.Station.FindAsync(id);
            if (station == null)
            {
                return NotFound();
            }

            context.Station.Remove(station);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool StationExists(int id)
        {
            return (context.Station?.Any(e => e.Id == id)).GetValueOrDefault();
        }   
    }
}
