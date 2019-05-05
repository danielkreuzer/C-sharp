using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wetr.Domain {
    public class GeoCoordinate {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }

        public GeoCoordinate(double latitude, double longitude, double altitude) {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        public GeoCoordinate(double latitude, double longitude) {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
