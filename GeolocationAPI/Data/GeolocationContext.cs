using GeolocationAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeolocationAPI.Data
{
    public class GeolocationContext: DbContext
    {

        public DbSet<Geolocation> Geolocation { get; set; }
        public DbSet<Language> Language { get; set; }
        public DbSet<Location> Location { get; set; }


        public GeolocationContext(DbContextOptions<GeolocationContext> options) : base(options)
        {

        }

        public GeolocationContext()
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Geolocation>().HasAlternateKey(g => g.IP);
            modelBuilder.Entity<Location>().HasAlternateKey(l => l.Geoname_id);
        }
    }
}
