using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dal.Common;
using Wetr.Dal.Interface;
using Wetr.Domain;

namespace Wetr.Dal.Ado {
    public class AdoProvinceDao : IProvinceDao {

        private static RowMapper<Province> provinceMapper =
                        row => new Province {
                            Id = (int)row["id"],
                            Name = (string)row["name"]
                        };

        private readonly AdoTemplate _template;

        public AdoProvinceDao(IConnectionFactory connectionFactory) {
            this._template = new AdoTemplate(connectionFactory);
        }

        public async Task<IEnumerable<Province>> FindAllAsync() {
            //Thread.Sleep(100000);
            return await _template.QueryAsync("select * from province", provinceMapper);
        }

        public async Task<Province> FindByIdAsync(int id) {
            return (await _template.QueryAsync("select * from province where id=@id",
                new[] { new QueryParameter("@id", id) },
                provinceMapper)).FirstOrDefault();
        }

        public async Task<IEnumerable<Province>> FindByNameAsync(string name) {
            return await _template.QueryAsync("select * from province where name=@name",
                new[] { new QueryParameter("@name", name) },
                provinceMapper);
        }

        public async Task<bool> AddProvinceAsync(Province province) {
            return await _template.ExecuteAsync(
                       "insert into province (name) values (@name)",
                       new[] {new QueryParameter("@name", province.Name)}
                   ) == 1;
        }

        public async Task<bool> UpdateProvinceAsync(Province province) {
            return await _template.ExecuteAsync(
                       "update province set name = @name where id = @id",
                       new[] {
                           new QueryParameter("@id", province.Id),
                           new QueryParameter("@name", province.Name)
                       }
                   ) == 1;
        }

        public async Task<bool> DeleteProvinceAsync(Province province) {
            return await _template.ExecuteAsync(
                       "delete from province where id = @id",
                       new[] { new QueryParameter("@id", province.Id) }
                   ) == 1;
        }
    }
}
