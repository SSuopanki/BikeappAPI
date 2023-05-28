using BikeappAPI.Models;

public interface IJourneysRepository 
{
    Task<IEnumerable<Journey>> GetAllJourneys();
    Task<Journey> GetJourneyById(Guid journeyId);
    Task CreateJourney(Journey journey);
    Task UpdateJourney(Journey journey);
    Task DeleteJourney(Guid journeyId);
    Task UploadJourneysFromCsv(IFormFile formfile);
}
