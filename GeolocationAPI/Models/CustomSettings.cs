using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeolocationAPI.Models
{
    public class CustomSettings
    {
        public string ApiKey { get; set; }
        public string EndPointipstackAPI { get; set; }

        private static CustomSettings instance = new CustomSettings();
        public static CustomSettings Instance => instance;
        
    }
}
