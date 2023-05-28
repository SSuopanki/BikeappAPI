using BikeappAPI.Models;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using CsvHelper;

namespace BikeappAPI.Repository
{
    public class StationsRepository : IStationsRepository
    {
        private readonly BikeappContext context;
        private readonly IConfiguration configuration;

        public StationsRepository(BikeappContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public async Task<IEnumerable<Station>> GetAllStations()
        {
            return await context.Station.ToListAsync();
        }

        public async Task<Station> GetStationById(int stationId)
        {
            var station = await context.Station.FindAsync(stationId);

            if (station == null)
            {
                //TODO: better null check
                Console.WriteLine("null station");
                return null;
            }

            return station;
        }

        public async Task CreateStation(Station station)
        {
            context.Station.Add(station);
            await context.SaveChangesAsync();
        }

        public async Task UpdateStation(Station station)
        {
            context.Entry(station).State = EntityState.Modified;
            await context.SaveChangesAsync();

        }

        public async Task DeleteStation(int stationId)
        {
            var station = await context.Station.FindAsync(stationId);
            if (station == null)
            {
                //TODO: better null check?
                Console.WriteLine("NotFound");
                return;
            }
            else
            {

                context.Station.Remove(station);
                await context.SaveChangesAsync();
            }
        }

        public async Task UploadStationsFromCsv(IFormFile formFile)
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

                csvReader.Context.RegisterClassMap<StationMap>();
                var records = csvReader.GetRecords<Station>().ToList();
                await context.Station.AddRangeAsync(records);
                context.SaveChanges();
            }

            return;
        }

        public class StationMap : ClassMap<Station>
        {
            public StationMap()
            {
                AutoMap(CultureInfo.InvariantCulture);
                Map(m => m.FId).Name("FID");
                Map(m => m.Id).Name("ID");
                Map(m => m.Nimi);
                Map(m => m.Namn);
                Map(m => m.Name);
                Map(m => m.Osoite);
                Map(m => m.Adress);
                Map(m => m.Kaupunki);
                Map(m => m.Stad);
                Map(m => m.Operaattor);
                Map(m => m.X).Name("x");
                Map(m => m.Y).Name("y");

            }
        }
    }
}
