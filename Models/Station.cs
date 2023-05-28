using System;
using System.Collections.Generic;

namespace BikeappAPI.Models
{
    public partial class Station
    {
        public int FId { get; set; }
        public int Id { get; set; }
        public string? Nimi { get; set; }
        public string? Namn { get; set; }
        public string? Name { get; set; }
        public string? Osoite { get; set; }
        public string? Adress { get; set; }
        public string? Kaupunki { get; set; }
        public string? Stad { get; set; }
        public string? Operaattor { get; set; }
        public int Kapasiteet { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
}