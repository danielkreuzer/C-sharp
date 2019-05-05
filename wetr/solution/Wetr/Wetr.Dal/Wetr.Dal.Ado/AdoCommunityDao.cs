using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dal.Common;
using Wetr.Dal.Interface;
using Wetr.Domain;

namespace Wetr.Dal.Ado {
    public class AdoCommunityDao : ICommunityDao {
        private static RowMapper<Community> communityMapper =
            row => new Community {
                Id = (int)row["id"],
                ZipCode = (int)row["zip_code"],
                Name = (string)row["name"],
                DistrictId = (int)row["district_id"],
                ZipName = (int)row["zip_code"] + " " + (string)row["name"]
            };

        private readonly AdoTemplate _template;

        public AdoCommunityDao(IConnectionFactory connectionFactory) {
            this._template = new AdoTemplate(connectionFactory);
        }

        public async Task<IEnumerable<Community>> FindAllAsync() {
            return await _template.QueryAsync("select * from community", communityMapper);
        }

        public async Task<IEnumerable<Community>> FindAllWithStationsAsync() {
            return await _template.QueryAsync("select * from community where id in (select community_id from station)", communityMapper);
        }

        public async Task<Community> FindByIdAsync(int id) {
            return (await _template.QueryAsync("select * from community where id=@id",
                new[] { new QueryParameter("@id", id) },
                communityMapper)).FirstOrDefault();
        }

        public async Task<IEnumerable<Community>> FindByZipCodeAsync(int zipCode) {
            return await _template.QueryAsync("select * from community where zip_code=@zip_code",
                new[] { new QueryParameter("@zip_code", zipCode) },
                communityMapper);
        }

        public async Task<IEnumerable<Community>> FindByNameAsync(string name) {
            return await _template.QueryAsync("select * from community where name=@name",
                new[] { new QueryParameter("@name", name) },
                communityMapper);
        }

        public async Task<IEnumerable<Community>> FindByDistrictIdAsync(int districtId) {
            return await _template.QueryAsync("select * from community where district_id=@district_id",
                new[] { new QueryParameter("@district_id", districtId) },
                communityMapper);
        }

        public async Task<bool> AddCommunityAsync(Community community) {
            return await _template.ExecuteAsync(
                       "insert into community (zip_code, name, district_id) values (@zip_code, @name, @district_id)",
                       new[] {
                           new QueryParameter("@zip_code", community.ZipCode),
                           new QueryParameter("@name", community.Name),
                           new QueryParameter("@district_id", community.DistrictId)
                       }
                   ) == 1;
        }

        public async Task<bool> UpdateCommunityAsync(Community community) {
            return await _template.ExecuteAsync(
                       "update community set zip_code = @zip_code, name = @name, district_id = @district_id where id = @id",
                       new[] {
                           new QueryParameter("@zip_code", community.ZipCode),
                           new QueryParameter("@name", community.Name),
                           new QueryParameter("@district_id", community.DistrictId),
                           new QueryParameter("@id", community.Id),
                       }
                   ) == 1;
        }

        public async Task<bool> DeleteCommunityAsync(Community community) {
            return await _template.ExecuteAsync(
                       "delete from community where id = @id",
                       new[] { new QueryParameter("@id", community.Id) }
                   ) == 1;
        }
    }
}
