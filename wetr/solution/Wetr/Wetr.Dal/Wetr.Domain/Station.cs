using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wetr.Domain {
    public class Station {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public StationType StationType { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CommunityId { get; set; }
        public Community Community { get; set; }
        public District District { get; set; }
        public Province Province { get; set; }
        public double Altitude { get; set; }
        public int Creator { get; set; }
        public User User { get; set; }

        public Station() {}

        public Station(int id, string name, int typeId, double latitude, double longitude, int communityId, double altitude, int creator) {
            Id = id;
            Name = name;
            TypeId = typeId;
            Latitude = latitude;
            Longitude = longitude;
            CommunityId = communityId;
            Altitude = altitude;
            Creator = creator;
            StationType = null;
            Community = null;
            District = null;
            Province = null;
            User = null;
        }

        public Station(string name, int typeId, double latitude, double longitude, int communityId, double altitude, int creator) {
            Name = name;
            TypeId = typeId;
            Latitude = latitude;
            Longitude = longitude;
            CommunityId = communityId;
            Altitude = altitude;
            Creator = creator;
            StationType = null;
            Community = null;
            District = null;
            Province = null;
            User = null;
        }

        public Station(int id, string name, StationType stationType, double latitude, double longitude, Community community, District district, Province province, double altitude, User user) {
            Id = id;
            Name = name;
            StationType = stationType;
            Latitude = latitude;
            Longitude = longitude;
            Community = community;
            District = district;
            Province = province;
            Altitude = altitude;
            User = user;
        }

        public GeoCoordinate GetGeoCoordinate() {
            return new GeoCoordinate(Latitude, Longitude, Altitude);
        }

        public Station(string name, int typeId, int communityId, int creator, GeoCoordinate geoCoordinate) {
            Name = name;
            TypeId = typeId;
            Latitude = geoCoordinate.Latitude;
            Longitude = geoCoordinate.Longitude;
            CommunityId = communityId;
            Altitude = geoCoordinate.Altitude;
            Creator = creator;
            StationType = null;
            Community = null;
            District = null;
            Province = null;
            User = null;
        }

        public Station(int id, string name, StationType stationType, Community community, District district, Province province, User user, GeoCoordinate geoCoordinate) {
            Id = id;
            Name = name;
            StationType = stationType;
            Latitude = geoCoordinate.Latitude;
            Longitude = geoCoordinate.Longitude;
            Community = community;
            District = district;
            Province = province;
            Altitude = geoCoordinate.Altitude;
            User = user;
        }
    }
}
