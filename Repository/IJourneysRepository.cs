using BikeappAPI.Models;

public interface IJourneysRepository 
{
    Task<IEnumerable<Journey>> GetAllJourneys();
    Task<Journey> GetJourneyById(int id);
    Task CreateJourney(Journey journey);
    Task UpdateJourney(Journey journey);
    Task DeleteJourney(int journeyId);
    Task UploadJourneysFromCsv(IFormFile formfile);
}
