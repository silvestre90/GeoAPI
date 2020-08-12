using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GeolocationAPI.Logging;
using GeolocationAPI.Models;
using GeolocationAPI.Repositories;
using GeolocationAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GeolocationAPI.Controllers
{
    [Route("api/geo")]
    [ApiController]
    public class GeolocationController : ControllerBase
    {
        private IOptions<CustomSettings> _settings;
        private IGeolocationRepository _repository;
        private ILog _logger;


        public GeolocationController(IGeolocationRepository geolocationRepository, IOptions<CustomSettings> settings, ILog logger)
        {
            _repository = geolocationRepository;
            _settings = settings;
            CustomSettings singleton = CustomSettings.Instance;
            singleton.ApiKey = _settings.Value.ApiKey;
            singleton.EndPointipstackAPI = _settings.Value.EndPointipstackAPI;
            _logger = logger;
        }

        [HttpGet]
        [Route("IP")]
        public async Task<IActionResult> GetGeoByIP(string ipAddress = null)
        {
            string methodName = MethodBase.GetCurrentMethod().ToString();

            if (Helper.IsIPAddressValid(ipAddress))
            {
                string message = $"{methodName} - Incorrect IP address";
                _logger.Information(message);
                return BadRequest(message);
            }
 
            return GetGeolocationByIP(ipAddress);
           
        }

        [HttpGet]
        [Route("uri")]
        public async Task<IActionResult> GetGeoByURI(string uri)
        {
            string methodName = MethodBase.GetCurrentMethod().ToString();

            if (Helper.IsValidURI(uri))
            {
                string message = $"{methodName} - Incorrect URL";
                _logger.Information(message);
                return BadRequest(message);
            }

            return GetGeolocationByURI(uri);

        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> GetGeolocation(string ipAddress = null, string uri = null)
        {
            string methodName = MethodBase.GetCurrentMethod().ToString();

            if (ipAddress == null && uri == null || ipAddress != null && uri != null)
            {
                string message = $"{methodName} Invalid parameters provided. Only one value should be filled, second must be empty";
                _logger.Information(message);
                return BadRequest(message);
            }

            if (ipAddress != null)
            {
                return GetGeolocationByIP(ipAddress);
            }
            else
            {
                return GetGeolocationByURI(uri);
            }
            
        }


        [HttpGet]
        [Route("all")]
        public  IActionResult GetAllGeolocations()
        {
            string methodName = MethodBase.GetCurrentMethod().ToString();
            try
            {
                return Ok(_repository.GetAllGeolocations());
            }
            catch (Exception e)
            {
                _logger.Error($"{methodName} - Unexpected error occured - {e.Message}");
                return StatusCode(500);
            }
            
        }
            

        private IActionResult GetGeolocationByURI(string uri)
        {

            string methodName = MethodBase.GetCurrentMethod().ToString();
            if (!Helper.IsValidURI(uri))
            {
                string message = $"{methodName} Incorrect URL address provided {uri}";
                _logger.Information(message);
                return BadRequest(message);
            }

            if (!_repository.GeolocationExists(null,uri))
            {
                _logger.Information($"{methodName} requested resource doesn't exists");
                return NotFound();
            }

            try
            {
                var geolocation = _repository.GetGeolocationByUri(uri);
                return Ok(geolocation);
            }
            catch (Exception e)
            {
                _logger.Error($"{methodName} - {e.Message}");
                return StatusCode(500);
            }
        }

        private IActionResult GetGeolocationByIP(string ipAddress)
        {

            string methodName = MethodBase.GetCurrentMethod().ToString();
            if (!Helper.IsIPAddressValid(ipAddress))
            {
                string message = $"{methodName} Incorrect IP address provided {ipAddress}";
                _logger.Information(message);
                return BadRequest(message);
            }

            if (!_repository.GeolocationExists(ipAddress))
            {
                _logger.Information($"{methodName} requested resource doesn't exists");
                return NotFound();
            }

            try
            {
                var geolocation = _repository.GetGeolocationByIP(ipAddress);
                return Ok(geolocation);
            }
            catch (Exception e)
            {
                _logger.Error($"{methodName} - {e.Message}");
                return StatusCode(500);
            }
        }

 

        // POST api/values
        [HttpPost]
        [Route("addlocationByURI")]
        public async Task<IActionResult> AddGeolocationDataURI(string uriAddress)
        {
            _logger.Information("Controller method invoked");

            IIPStackClient _client = new IPStackClient();
            Geolocation geolocation = new Geolocation();

            try
            {
                geolocation = await _client.GetGeolocationByURIaddress(uriAddress);
            }
            catch (Exception e)
            {
                _logger.Error($"Unable to retrieve geolocation data. Reason - {e.Message}");
                return StatusCode(500, e);
            }

            if (geolocation.IP == null)
            {
                _logger.Warning("Empty object returned. No data will be added to repository");
                return StatusCode(500);
            }

            geolocation.URI = uriAddress;
            return TrySaveGeolocation(geolocation);
        }


        [HttpPost]
        [Route("addlocationByIP")]
        public async Task<IActionResult> AddGeolocationDataIP(string ipAddress)
        {
            string methodName = MethodBase.GetCurrentMethod().ToString();
            _logger.Information($"{methodName} Controller method invoked");
            if (!Helper.IsIPAddressValid(ipAddress))
            {
                string message = $"{methodName} Incorrect IP address provided {ipAddress}";
                _logger.Information(message);
                return BadRequest(message);
            }

            IIPStackClient _client = new IPStackClient();
            Geolocation geolocation = new Geolocation();

            try
            {
                geolocation = await _client.GetGeolocationByIPAddress(ipAddress);
            }
            catch (Exception e)
            {
                _logger.Error($"Unable to retrieve geolocation data. Reason - {e.Message}");
                return StatusCode(500, e);
            }

            if (geolocation.IP == null)
            {
                _logger.Warning("Empty object returned. No data will be added to repository");
                return StatusCode(500);
            }

            return TrySaveGeolocation(geolocation);
        }

        private IActionResult TrySaveGeolocation(Geolocation geolocation)
        {
            try
            {
                _logger.Information($"Attempt to save record with ip {geolocation.IP}");
                _repository.AddGeolocationData(geolocation);
            }
            catch (Exception e)
            {
                _logger.Error($"Error when saving data in database - {e.Message}");
                _logger.Information(e.Source);
                return StatusCode(500);
            }

            return Ok();
        }

        // DELETE api/geo/onet.pl
        [HttpDelete]
        [Route("delete/uri")]
        public IActionResult DeleteGeolocationByUri(string uri)
        {
            string methodName = MethodBase.GetCurrentMethod().ToString();
            string message;
            _logger.Information($"{methodName} method invoked");

            if (!Helper.IsValidURI(uri))
            {
                message = $"{methodName}. Provided URI address is invalid - {uri}";
                _logger.Information(message);
                return BadRequest(message);
            }

            var geolocationToDelete = _repository.GetGeolocationByUri(uri);
            if (geolocationToDelete == null)
            {
                message = $"{methodName} Requested resource doesn't exists. Uri - {uri}";
                _logger.Information(message);
                return NotFound(message);
            }
            else
            {
                try
                {
                    _repository.DeleteGeolocation(geolocationToDelete);
                }
                catch (Exception e)
                {
                    _logger.Warning($"{methodName} Failure when removing geolocation with Ip {geolocationToDelete.IP}. {e.Message} ");
                    return StatusCode(500, $"Unexpected error when trying to delete resource {geolocationToDelete.IP}");
                }
            }

            return Ok("Element removed");
        }

        // DELETE api/geo/11.22.333.456
        [HttpDelete]
        [Route("delete/ip")]
        public IActionResult DeleteGeolocationByIP(string ip)
        {
            string methodName = MethodBase.GetCurrentMethod().ToString();
            _logger.Information($"{methodName} method invoked");


            if (!Helper.IsIPAddressValid(ip))
            {
                string message = $"{methodName} Incorrect IP address provided {ip}";
                _logger.Information(message);
                return BadRequest(message);
            }

            var geolocationToDelete = _repository.GetGeolocationByIP(ip);
            if (geolocationToDelete == null)
            {
                string message = $"{methodName} Requested resource doesn't exists. IP - {ip}";
                _logger.Information(message);
                return NotFound(message);
            }
            else
            {
                try
                {
                    _repository.DeleteGeolocation(geolocationToDelete);
                    
                }
                catch (Exception e)
                {
                    _logger.Warning($"{methodName} Failure when removing geolocation with Ip {geolocationToDelete.IP}. {e.Message} ");
                    return StatusCode(500, $"Unexpected error when trying to delete resource {geolocationToDelete.IP}");
                }
            }

            return Ok("Element removed");
        }
    }
}
