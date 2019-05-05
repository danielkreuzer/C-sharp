using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dal.Common;
using Wetr.Dal.Interface;
using Wetr.Domain;

namespace Wetr.Dal.Ado {
    public class AdoDistrictDao : IDistrictDao {

        private static RowMapper<District> districtMapper =
            row => new District {
                Id = (int)row["id"],
                Name = (string)row["name"],
                ProvinceId = (int)row["province_id"]
            };

        private readonly AdoTemplate _template;

        public AdoDistrictDao(IConnectionFactory connectionFactory) {
            this._template = new AdoTemplate(connectionFactory);
        }

        public async Task<IEnumerable<District>> FindAllAsync() {
            return await _template.QueryAsync("select * from district", districtMapper);
        }

        public async Task<District> FindByIdAsync(int id) {
            return (await _template.QueryAsync("select * from district where id=@id",
                new[] { new QueryParameter("@id", id) },
                districtMapper)).FirstOrDefault();
        }

        public async Task<IEnumerable<District>> FindByNameAsync(string name) {
            return await _template.QueryAsync("select * from district where name=@name",
                new[] { new QueryParameter("@name", name) },
                districtMapper);
        }

        public async Task<IEnumerable<District>> FindByProvinceIdAsync(int provinceId) {
            return await _template.QueryAsync("select * from district where province_id=@province_id",
                new[] { new QueryParameter("@province_id", provinceId) },
                districtMapper);
        }

        public async Task<bool> AddDistrictAsync(District district) {
            return await _template.ExecuteAsync(
                       "insert into district (name, province_id) values (@name, @province_id)",
                       new[] {
                           new QueryParameter("@name", district.Name),
                           new QueryParameter("@province_id", district.ProvinceId) 
                       }
                   ) == 1;
        }

        public async Task<bool> UpdateDistrictAsync(District district) {
            return await _template.ExecuteAsync(
                       "update district set name = @name, province_id = @province_id where id = @id",
                       new[] {
                           new QueryParameter("@id", district.Id),
                           new QueryParameter("@name", district.Name),
                           new QueryParameter("@province_id", district.ProvinceId)
                       }
                   ) == 1;
        }

        public async Task<bool> DeleteDistrictAsync(District district) {
            return await _template.ExecuteAsync(
                       "delete from district where id = @id",
                       new[] { new QueryParameter("@id", district.Id) }
                   ) == 1;
        }
    }
}
