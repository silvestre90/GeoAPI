using GeolocationAPI.Logging;
using GeolocationAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GeolocationAPI.Services
{
    public class IPStackClient : IIPStackClient
    {
        private static HttpClient _httpClient = new HttpClient();
        private CustomSettings _settings;
        private LogNLog _logger;

        public IPStackClient()
        {
            _settings = CustomSettings.Instance;
            _logger = new LogNLog();
        }
        
        public async Task<Geolocation> GetGeolocationByURIaddress(string uri)
        {
            
            _logger.Information($"GetGeolocationByURIaddress invoked with URI address {uri}");
            try
            {
                Geolocation geolocation = await GetGeolocation(uri);
                return geolocation;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Geolocation> GetGeolocationByIPAddress(string ipAddress)
        {
            
            _logger.Information($"GetGeolocationByIPAddress invoked with IP address {ipAddress}");
            try
            {
                Geolocation geolocation = await GetGeolocation(ipAddress);
                return geolocation;
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        private async Task<Geolocation> GetGeolocation(string address)
        {    
            try
            {
                string uri = Helper.GetIpStackEndPoint(address);
                var response = await _httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                Geolocation geolocation = new Geolocation();
                if (!response.IsSuccessStatusCode)
                {
                    _logger.Warning($"Respone of request {uri} returned error. {response.StatusCode} - {response.RequestMessage}");
                    throw new Exception("Unable to access the resource");
                }
                else
                {
                    geolocation = JsonConvert.DeserializeObject<Geolocation>(content);
                }

                if (geolocation.IP == null)
                {
                    var errors = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                    var message = errors["error"];
                    _logger.Error(message.ToString());
                    throw new Exception("Invalid Json content returned");
                }
                return geolocation;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw e;
            }
             
        }
    }
}
