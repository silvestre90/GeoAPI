using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GeolocationAPI.Data;
using GeolocationAPI.Logging;
using GeolocationAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GeolocationAPI.Repositories
{
    public class GeolocationRepository : IGeolocationRepository
    {
        //TODO - Validator adresów IP
        //funkcjonalność nieprzyjmowania duplikatów do żadnej z tabel
        //zwracanie odpowiednich wartości z metod API
        //dodanie NLoga i trackowanie zdarzeń aplikacji

        private GeolocationContext _context;
        private ILog _logger;

        public GeolocationRepository(GeolocationContext context)
        {
            _context = context;
            _logger = new LogNLog();
        }

        public void AddGeolocationData(Geolocation geolocation)
        {
            if (GetGeolocationByIP(geolocation.IP) != null)
            {
                _logger.Information($"Provided geolocation data already exists for IP address {geolocation.IP}. New record won't be saved");
                return;
            }


            geolocation.Location = DetermineLocation(geolocation.Location);

            try
            {
                _context.Geolocation.Add(geolocation);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.Error($"AddGeolocationData threw an error while processing request with IP {geolocation.IP}. {e.Message}");
                throw e;
            }
                
        }

        public IEnumerable<Language> GetAllLanguages()
        {
            return _context.Language.ToList();
        }

        private List<Language> ReturnLanguagesToAdd(List<Language> languages)
        {
            var uniqueRecords = new List<Language>();
            var languagesDb = GetAllLanguages();
            foreach (var lang in languages)
            {
                int counter = languagesDb.Where(l => l.Name == lang.Name && l.Native == lang.Native && l.Code == lang.Code).ToList().Count;

                if(counter == 0)
                {
                    uniqueRecords.Add(lang);
                }
                

            }
            return uniqueRecords;
        }

        private Location DetermineLocation(Location location)
        {
            var locationDb = _context.Location.SingleOrDefault(l => l.Geoname_id == location.Geoname_id);

            if(locationDb != null)
            {
                return locationDb;
            }
            else
            {
                return location;
            }
        }

        public void DeleteGeolocationData(string IPAddress)
        {
            var geolocationInDb = _context.Geolocation.SingleOrDefault(g => g.IP == IPAddress);
        }

        public Geolocation GetGeolocationByIP(string IPAddress)
        {
            var geolocation = _context.Geolocation
                .Where(g => g.IP == IPAddress)
                .Include(l => l.Location)
                .Include(lg => lg.Location.Languages)
                .SingleOrDefault();

            return  geolocation;
        }

        public bool GeolocationExists(string ip = null, string uri = null)
        {
            string methodName = MethodBase.GetCurrentMethod().ToString();
            if ( ip == null && uri == null)
            {
                _logger.Information($"{methodName} - no arguments provided");
                return false;
            }else if(ip != null && uri != null)
            {
                _logger.Information($"{methodName} - too many arguments provided. Either IP or URL must be empty");
                return false;
            }

            var geolocationList = GetAllGeolocations();
            if(ip != null)
            {
                return geolocationList.Where(g => g.IP == ip).ToList().Count > 0;
            }
            else
            {
                return geolocationList.Where(g => g.URI == uri).ToList().Count > 0;
            }
            
        }

        public IEnumerable<Geolocation> GetAllGeolocations()
        {
            return _context.Geolocation
                    .Include(l => l.Location)
                    .Include(lg => lg.Location.Languages)
                    .ToList();
        }

        public void DeleteGeolocation(Geolocation geolocation)
        {
            string methodName = MethodBase.GetCurrentMethod().ToString();

            if (GeolocationExists(geolocation.IP))
            {
                _logger.Information($"{methodName} - geolocation found in database. Delete attempt");
                try
                {
                    _context.Geolocation.Remove(geolocation);
                    _context.SaveChanges();
                    
                }
                catch (Exception e)
                {
                    _logger.Error($"{MethodBase.GetCurrentMethod()} - Unable to delete Geolocation with IP {geolocation.IP}");
                    throw e;
                }

            }
            else
            {
                _logger.Information($"{methodName} - Unable to find geolocation with provided IP address {geolocation.IP}");
            }
            
        }

        public Geolocation GetGeolocationByUri(string uri)
        {
            var geolocation = _context.Geolocation.Where(g => g.URI == uri)
                                                  .Include(l => l.Location)
                                                  .Include(lg => lg.Location.Languages)
                                                  .SingleOrDefault();
            return geolocation;
        }

        public Location GetLocationById(int id)
        {
            var location = _context.Location.SingleOrDefault(l => l.Id == id);
            return location;
        }
    }
}
