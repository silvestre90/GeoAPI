using GeolocationAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeolocationAPI.Services
{
    public interface IIPStackClient
    {
        Task<Geolocation> GetGeolocationByIPAddress(string ipAddress);
        Task<Geolocation> GetGeolocationByURIaddress(string uri);
    }
}
