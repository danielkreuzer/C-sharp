using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dal.Common;
using Wetr.Dal.Interface;
using Wetr.Domain;

namespace Wetr.Dal.Ado {
    public class AdoStationDao : IStationDao {
        private static readonly RowMapper<Station> StationMapper =
            row => new Station {
                Id = (int)row["id"],
                Name = (string)row["name"],
                TypeId = (int)row["type_id"],
                Latitude = (float)row["latitude"],
                Longitude = (float)row["longitude"],
                Altitude = (float)row["altitude"],
                CommunityId = (int)row["community_id"],
                Creator = (int)row["creator"],
                Community = null,
                District = null,
                Province = null,
                User = null
            };

        private static readonly RowMapper<Station> StationJoinedMapper =
            row => new Station {
                Id = (int) row["id"],
                Name = (string) row["name"],
                StationType = new StationType {
                    Id = (int)row["stid"],
                    Manufacturer = (string)row["manufacturer"],
                    Model = (string)row["model"]
                },
                Latitude = (float) row["latitude"],
                Longitude = (float) row["longitude"],
                Altitude = (float)row["altitude"],
                Community = new Community {
                    Id = (int)row["cid"],
                    ZipCode = (int)row["zip_code"],
                    Name = (string)row["cname"],
                    DistrictId = (int)row["did"]
                },
                District = new District {
                    Id = (int)row["did"],
                    Name = (string)row["dname"],
                    ProvinceId = (int)row["pid"]
                },
                Province = new Province {
                    Id = (int)row["pid"],
                    Name = (string)row["pname"]
                },
                User = new User {
                    Id = (int)row["uid"],
                    Username = (string)row["username"],
                    Password = (string)row["password"],
                    Email = (string)row["email"],
                    FirstName = (string)row["first_name"],
                    LastName = (string)row["last_name"],
                    CommunityId = (int)row["ucommunity_id"],
                    DateOfBirth = (DateTime)row["date_of_birth"]
                }
            };

        private readonly AdoTemplate _template;

        public AdoStationDao(IConnectionFactory connectionFactory) {
            this._template = new AdoTemplate(connectionFactory);
        }

        public async Task<Station> FindByIdAsync(int id) {
            return (await _template.QueryAsync("SELECT * FROM station WHERE id =@id",
                new[] { new QueryParameter("@id", id) }, StationMapper)).FirstOrDefault();
        }

        public async Task<Station> FindJoinedByIdAsync(int id) {
            return (await _template.QueryAsync("SELECT station.id AS \"id\", station.name AS \"name\", station.latitude, station.longitude, station.altitude, st.id AS \"stid\",\r\n       st.manufacturer AS \"manufacturer\", st.model AS \"model\",\r\n       c.id AS \"cid\", c.name AS \"cname\", c.zip_code AS \"zip_code\",\r\n       d.id AS \"did\", d.name AS \"dname\", p.id AS \"pid\", p.name AS \"pname\",\r\n       u.id AS \"uid\", u.community_id AS \"ucommunity_id\", u.date_of_birth,\r\n       u.first_name, u.last_name, u.email, u.password, u.username\r\n  FROM station\r\n    JOIN community c on station.community_id = c.id\r\n    JOIN district d on c.district_id = d.id\r\n    JOIN province p on d.province_id = p.id\r\n    JOIN station_type st on station.type_id = st.id\r\n    JOIN user u on station.creator = u.id\r\n  WHERE station.id =@id",
                new[] { new QueryParameter("@id", id) }, StationJoinedMapper)).FirstOrDefault();
        }

        public async Task<IEnumerable<Station>> FindJoinedAsync() {
            return (await _template.QueryAsync("SELECT station.id AS \"id\", station.name AS \"name\", station.latitude, station.longitude, station.altitude, st.id AS \"stid\",\r\n       st.manufacturer AS \"manufacturer\", st.model AS \"model\",\r\n       c.id AS \"cid\", c.name AS \"cname\", c.zip_code AS \"zip_code\",\r\n       d.id AS \"did\", d.name AS \"dname\", p.id AS \"pid\", p.name AS \"pname\",\r\n       u.id AS \"uid\", u.community_id AS \"ucommunity_id\", u.date_of_birth,\r\n       u.first_name, u.last_name, u.email, u.password, u.username\r\n  FROM station\r\n    JOIN community c on station.community_id = c.id\r\n    JOIN district d on c.district_id = d.id\r\n    JOIN province p on d.province_id = p.id\r\n    JOIN station_type st on station.type_id = st.id\r\n    JOIN user u on station.creator = u.id\r\n",
                 StationJoinedMapper));
        }

        public async Task<IEnumerable<Station>> FindAllAsync() {
            return await _template.QueryAsync("SELECT * FROM station", StationMapper);
        }

        public async Task<IEnumerable<Station>> FindByNameAsync(string name) {
            return await _template.QueryAsync("SELECT * FROM station WHERE name =@name",
                new[] { new QueryParameter("@name", name) }, StationMapper);
        }

        public async Task<IEnumerable<Station>> FindByStationTypeIdAsync(int stationTypeId) {
            return await _template.QueryAsync("SELECT * FROM station WHERE type_id =@type_id",
                new[] { new QueryParameter("@type_id", stationTypeId) }, StationMapper);
        }

        public async Task<IEnumerable<Station>> FindByCommunityIdAsync(int communityId) {
            return await _template.QueryAsync("SELECT * FROM station WHERE community_id =@communityId",
                new[] { new QueryParameter("@communityId", communityId) }, StationMapper);
        }

        public async Task<IEnumerable<Station>> FindByDistrictIdAsync(int districtId) {
            return await _template.QueryAsync("SELECT * FROM station JOIN community c on station.community_id = c.id WHERE district_id = @communityId",
                new[] { new QueryParameter("@communityId", districtId) }, StationMapper);
        }

        public async Task<IEnumerable<Station>> FindByProvinceIdAsync(int provinceId) {
            return await _template.QueryAsync("SELECT * FROM station JOIN community c on station.community_id = c.id JOIN district d on c.district_id = d.id WHERE province_id = @communityId",
                new[] { new QueryParameter("@communityId", provinceId) }, StationMapper);
        }

        public async Task<IEnumerable<Station>> FindByCreatorIdAsync(int creatorId) {
            return await _template.QueryAsync("SELECT * FROM station WHERE creator = @creator",
                new[] {new QueryParameter("@creator", creatorId)}, StationMapper);
        }


        public async Task<bool> UpdateAllAsync(Station station) {
            return await _template.ExecuteAsync("UPDATE station SET name = @name, type_id = @type_id, latitude = @latitude, longitude = @longitude, community_id = @community_id, altitude = @altitude, creator = @creator WHERE id = @id",
                new[] {
                    new QueryParameter("@name", station.Name),
                    new QueryParameter("@type_id", station.TypeId),
                    new QueryParameter("@latitude", station.Latitude),
                    new QueryParameter("@longitude", station.Longitude),
                    new QueryParameter("@community_id", station.CommunityId),
                    new QueryParameter("@altitude", station.Altitude),
                    new QueryParameter("@creator", station.Creator), 
                    new QueryParameter("@id", station.Id)
                }) == 1;
        }

        public async Task<bool> AddStationAsync(Station station) {
            return await _template.ExecuteAsync("INSERT INTO station (name, type_id, latitude, longitude, community_id, altitude, creator) " +
                                                "VALUES (@name, @type_id, @latitude, @longitude, @community_id, @altitude, @creator)",
                       new[] {
                           new QueryParameter("@name", station.Name),
                           new QueryParameter("@type_id", station.TypeId),
                           new QueryParameter("@latitude", station.Latitude),
                           new QueryParameter("@longitude", station.Longitude),
                           new QueryParameter("@community_id", station.CommunityId),
                           new QueryParameter("@altitude", station.Altitude),
                           new QueryParameter("@creator", station.Creator)
                       }) == 1;
        }

        public async Task<bool> DeleteStationAsync(Station station) {
            return await _template.ExecuteAsync("DELETE FROM station WHERE id = @id",
                       new[] {
                           new QueryParameter("@id", station.Id)
                       }) == 1;
        }
    }
}
