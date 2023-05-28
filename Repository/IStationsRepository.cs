using BikeappAPI.Models;

public interface IStationsRepository
{
    Task<IEnumerable<Station>> GetAllStations();
    Task<Station> GetStationById(int stationId);
    Task CreateStation(Station station);
    Task UpdateStation(Station station);
    Task DeleteStation(int stationId);
    Task UploadStationsFromCsv(IFormFile formFile);
}

