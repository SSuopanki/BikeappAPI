using BikeappAPI.Models;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using CsvHelper.TypeConversion;
using CsvHelper;

namespace BikeappAPI.Repositories
{
    public class JourneysRepository : IJourneysRepository
    {
        private readonly BikeappContext context;
        private readonly IConfiguration configuration;

        public JourneysRepository(BikeappContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public async Task<IEnumerable<Journey>> GetAllJourneys()
        {
            return await context.Journey.ToListAsync();
        }

        public async Task<Journey> GetJourneyById(Guid journeyId)
        {
            var journey = await context.Journey.FindAsync(journeyId);

            if (journey == null)
            {
                //TODO: better null check
                Console.WriteLine("null journey");
                return null;
            }

            return journey;
        }

        public async Task CreateJourney(Journey journey)
        {
            context.Journey.Add(journey);
            await context.SaveChangesAsync();
        }


        public async Task UpdateJourney(Journey journey)
        {
            context.Entry(journey).State = EntityState.Modified;
            await context.SaveChangesAsync();

        }


        public async Task DeleteJourney(Guid journeyId)
        {
            var journey = await context.Journey.FindAsync(journeyId);
            if (journey == null)
            {
                //TODO: better null check?
                Console.WriteLine("NotFound");
                return;
            }
            else
            {

                context.Journey.Remove(journey);
                await context.SaveChangesAsync();
            }
        }

        public async Task UploadJourneysFromCsv(IFormFile formFile)
        {
            var data = new MemoryStream();
            await formFile.CopyToAsync(data);

            data.Position = 0;

            var bad = new List<string>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null,
                MissingFieldFound = null,
                BadDataFound = context =>
                {
                     bad.Add(context.RawRecord);
                }
            };
            using (TextReader reader = new StreamReader(data))
            using (var csvReader = new CsvReader(reader, config))
            {

                csvReader.Context.RegisterClassMap<JourneyMap>();
                var records = csvReader.GetRecords<Journey>().ToList();
                foreach(Journey journey in records)
                {
                    journey.JourneyId = Guid.NewGuid();
                }
                await context.Journey.AddRangeAsync(records);
                context.SaveChanges();
            }

            return;
        }

        public class JourneyMap : ClassMap<Journey>
        {
            public JourneyMap()
            {
                AutoMap(CultureInfo.InvariantCulture);
                Map(m => m.DepartureDate).Name("Departure");
                Map(m => m.ReturnDate).Name("Return");
                Map(m => m.DepartureStationId).Name("Departure station id");
                Map(m => m.DepartureStationName).Name("Departure station name");
                Map(m => m.ReturnStationId).Name("Return station id");
                Map(m => m.ReturnStationName).Name("Return station name");
                Map(m => m.Distance).Name("Covered distance (m)").TypeConverter<DecimalConverter>();
                Map(m => m.Duration).Name("Duration (sec.)").TypeConverter<Int32Converter>();
            }
        }
    }
}
