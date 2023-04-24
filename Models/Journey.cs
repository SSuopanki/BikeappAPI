using System;
using System.Collections.Generic;

namespace BikeappAPI.Models
{
    public partial class Journey
    {
        public int JourneyId { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int DepartureStationId { get; set; }
        public string? DertureStationName { get; set; }
        public int ReturnStationId { get; set; }
        public string? ReturnStationName { get; set; }
        public int Distance { get; set; }
        public int Duration { get; set; }
    }
}