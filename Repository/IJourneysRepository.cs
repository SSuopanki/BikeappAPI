using BikeappAPI.Models;
using BikeappAPI.Repository;

public interface IJourneysRepository 
{
    Task<IEnumerable<Journey>> GetAllJourneys();
    Task<Journey> GetJourneyById(int id);
    Task CreateJourney(Journey journey);
    Task UpdateJourney(Journey dbJourney, Journey journey);
    Task DeleteJourney(int journeyId);
    Task UploadJourneysFromCsv(IFormFile file);
}
