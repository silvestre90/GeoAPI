using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GeolocationAPI.Models;
using Microsoft.Extensions.Options;

namespace GeolocationAPI.Services
{
    public static class Helper
    {
        public static string GetIpStackEndPoint(string ipAddress)
        {
            CustomSettings settings = CustomSettings.Instance;
            char splitter = '?';

            string[] baseUri = settings.EndPointipstackAPI.Split(splitter);
            return $"{baseUri[0]}{ipAddress}{splitter}{baseUri[1]}{settings.ApiKey}";
        }

        public static bool IsIPAddressValid(string IPAddress)
        {
            if(IPAddress == null || IPAddress.Length == 0)
            {
                return false;
            }
            return Regex.IsMatch(IPAddress, "^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
        }

        public static bool IsValidURI(string URI)
        {
            if (URI == null || URI.Length == 0)
            {
                return false;
            }
            string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return Rgx.IsMatch(URI);
        }
    }
}
