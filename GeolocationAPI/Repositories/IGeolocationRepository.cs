using GeolocationAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeolocationAPI.Repositories
{
    public interface IGeolocationRepository
    {
        void AddGeolocationData(Geolocation geolocation);
        void DeleteGeolocationData(string IPAddress);
        Geolocation GetGeolocationByIP(string IPAddress);
        IEnumerable<Language> GetAllLanguages();
        IEnumerable<Geolocation> GetAllGeolocations();
        bool GeolocationExists(string ip = null,string uri = null);
        void DeleteGeolocation(Geolocation geolocation);
        Geolocation GetGeolocationByUri(string uri);
        Location GetLocationById(int i);
    }
}
