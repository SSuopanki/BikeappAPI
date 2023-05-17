using BikeappAPI.Models;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.Data.SqlClient;
using System.Data;
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

        public async Task<Journey> GetJourneyById(int journeyId)
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


        public async Task DeleteJourney(int journeyId)
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

        public async Task UploadJourneysFromCsv(List<Journey> journeys)
        {
            string connectionString = configuration.GetConnectionString("Bikeapp");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                        {

                            bulkCopy.DestinationTableName = "Journey";

                            bulkCopy.ColumnMappings.Add(nameof(Journey.JourneyId), "JourneyId");
                            bulkCopy.ColumnMappings.Add(nameof(Journey.DepartureDate), "Departure");
                            bulkCopy.ColumnMappings.Add(nameof(Journey.ReturnDate), "Return");
                            bulkCopy.ColumnMappings.Add(nameof(Journey.DepartureStationId), "Departure station id");
                            bulkCopy.ColumnMappings.Add(nameof(Journey.DepartureStationName), "Departure station name");
                            bulkCopy.ColumnMappings.Add(nameof(Journey.ReturnStationId), "Return station id");
                            bulkCopy.ColumnMappings.Add(nameof(Journey.ReturnStationName), "Return station name");
                            bulkCopy.ColumnMappings.Add(nameof(Journey.Distance), "Covered distance (m)");
                            bulkCopy.ColumnMappings.Add(nameof(Journey.Duration), "Duration (sec.)");


                            // Create a DataTable from the list of journeys
                            var dt = new DataTable();
                            dt.Columns.Add(nameof(Journey.JourneyId), typeof(Guid));
                            dt.Columns.Add(nameof(Journey.DepartureDate), typeof(DateTime));
                            dt.Columns.Add(nameof(Journey.ReturnDate), typeof(DateTime));
                            dt.Columns.Add(nameof(Journey.DepartureStationId), typeof(int));
                            dt.Columns.Add(nameof(Journey.DepartureStationName), typeof(string));
                            dt.Columns.Add(nameof(Journey.ReturnStationId), typeof(int));
                            dt.Columns.Add(nameof(Journey.ReturnStationName), typeof(string));
                            dt.Columns.Add(nameof(Journey.Distance), typeof(decimal));
                            dt.Columns.Add(nameof(Journey.Duration), typeof(int));

                            foreach (var journey in journeys)
                            {
                                var row = dt.NewRow();
                                row[nameof(Journey.JourneyId)] = journey.JourneyId;
                                row[nameof(Journey.DepartureDate)] = journey.DepartureDate;
                                row[nameof(Journey.ReturnDate)] = journey.ReturnDate;
                                row[nameof(Journey.DepartureStationId)] = journey.DepartureStationId;
                                row[nameof(Journey.DepartureStationName)] = journey.DepartureStationName;
                                row[nameof(Journey.ReturnStationId)] = journey.ReturnStationId;
                                row[nameof(Journey.ReturnStationName)] = journey.ReturnStationName;
                                row[nameof(Journey.Distance)] = journey.Distance;
                                row[nameof(Journey.Duration)] = journey.Duration;

                                dt.Rows.Add(row);
                            }
                            await bulkCopy.WriteToServerAsync(dt);
                        }

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        // Handle exception and rollback the transaction if necessary
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
