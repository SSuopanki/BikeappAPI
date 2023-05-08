﻿using BikeappAPI.Models;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BikeappAPI.Repositories
{
    public class JourneysRepository : IJourneysRepository
    {
        private readonly BikeappContext context;

        public JourneysRepository(BikeappContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Journey>> GetAllJourneys()
        {
            return await context.Journey.ToListAsync();
        }

        public async Task<Journey> GetJourneyById(int journeyId)
        {
            return await context.Journey.FindAsync(journeyId);
        }

        public async Task CreateJourney(Journey journey)
        {
            context.Journey.Add(journey);
            await context.SaveChangesAsync();
        }
        public async Task UpdateJourney(Journey dbJourney, Journey journey)
        {
            context.Entry(journey).State = EntityState.Modified;
            await context.SaveChangesAsync();

        }


        public async Task DeleteJourney(int journeyId)
        {
            var journey = await context.Journey.FindAsync(journeyId);
            context.Journey.Remove(journey);
            await context.SaveChangesAsync();
        }

        public async Task UploadJourneysFromCsv(IFormFile file)
        {
            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                };

                var csvReader = new CsvReader(streamReader, csvConfig);

                // Read the CSV file and convert each row to a Journey object
                var journeys = csvReader.GetRecords<Journey>().ToList();

                // Add the new journeys to the database
                foreach (var journey in journeys)
                {
                    context.Journey.Add(journey);
                }
                await context.SaveChangesAsync();
            }
        }

        internal Task UpdateJourney(Journey journey)
        {
            throw new NotImplementedException();
        }
    }
}