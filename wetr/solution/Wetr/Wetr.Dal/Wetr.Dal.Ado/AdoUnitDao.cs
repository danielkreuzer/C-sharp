using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dal.Common;
using Wetr.Dal.Interface;
using Wetr.Domain;

namespace Wetr.Dal.Ado {
    public class AdoUnitDao : IUnitDao {
        private static RowMapper<Unit> unitMapper =
            row => new Unit {
                Id = (int)row["id"],
                ShortName = (string)row["short_name"],
                LongName = (string)row["long_name"]
            };

        private readonly AdoTemplate _template;

        public AdoUnitDao(IConnectionFactory connectionFactory) {
            this._template = new AdoTemplate(connectionFactory);
        }

        public async Task<IEnumerable<Unit>> FindAllAsync() {
            return await _template.QueryAsync("select * from unit", unitMapper);
        }

        public async Task<Unit> FindByIdAsync(int id) {
            return (await _template.QueryAsync("select * from unit where id=@id",
                new[] { new QueryParameter("id", id) },
                unitMapper)).FirstOrDefault();
        }

        public async Task<IEnumerable<Unit>> FindByShortNameAsync(string name) {
            return await _template.QueryAsync("select * from unit where short_name=@short_name",
                new[] { new QueryParameter("short_name", name) },
                unitMapper);
        }

        public async Task<IEnumerable<Unit>> FindByLongNameAsync(string name) {
            return await _template.QueryAsync("select * from unit where long_name=@long_name",
                new[] { new QueryParameter("long_name", name) },
                unitMapper);
        }

        public async Task<bool> AddUnitAsync(Unit unit) {
            return await _template.ExecuteAsync(
                       "insert into unit (short_name, long_name) values (@short_name, @long_name)",
                       new[] {
                           new QueryParameter("@short_name", unit.ShortName),
                           new QueryParameter("@long_name", unit.LongName),
                       }
                   ) == 1;
        }

        public async Task<bool> UpdateUnitAsync(Unit unit) {
            return await _template.ExecuteAsync(
                       "update unit set short_name = @short_name, long_name = @long_name where id = @id",
                       new[] {
                           new QueryParameter("@short_name", unit.ShortName),
                           new QueryParameter("@long_name", unit.LongName),
                           new QueryParameter("@id", unit.Id), 
                       }
                   ) == 1;
        }

        public async Task<bool> DeleteUnitAsync(Unit unit) {
            return await _template.ExecuteAsync(
                       "delete from unit where id = @id",
                       new[] { new QueryParameter("@id", unit.Id) }
                   ) == 1;
        }
    }
}
