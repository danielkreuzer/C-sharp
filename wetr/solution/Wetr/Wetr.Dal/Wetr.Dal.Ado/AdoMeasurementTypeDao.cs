using Dal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Dal.Interface;
using Wetr.Domain;

namespace Wetr.Dal.Ado {
    public class AdoMeasurementTypeDao : IMeasurementTypeDao {
        private static RowMapper<MeasurementType> measurementTypeMapper =
            row => new MeasurementType {
                Id = (int)row["id"],
                Name = (string)row["name"]
            };

        private readonly AdoTemplate _template;

        public AdoMeasurementTypeDao(IConnectionFactory connectionFactory) {
            this._template = new AdoTemplate(connectionFactory);
        }

        public async Task<IEnumerable<MeasurementType>> FindAllAsync() {
            return await _template.QueryAsync("select * from measurement_type", measurementTypeMapper);
        }

        public async Task<MeasurementType> FindByIdAsync(int id) {
            return (await _template.QueryAsync("select * from measurement_type where id=@id",
                new[] { new QueryParameter("@id", id) },
                measurementTypeMapper
            )).FirstOrDefault();
        }

        public async Task<MeasurementType> FindByNameAsync(string name) {
            return (await _template.QueryAsync("select * from measurement_type where name=@name",
                new[] { new QueryParameter("@name", name) },
                measurementTypeMapper
            )).FirstOrDefault();
        }

        public async Task<bool> AddMeasurementTypeAsync(MeasurementType measurementType) {
            return await _template.ExecuteAsync(
                       "INSERT INTO measurement_type (name) VALUES (@name)",
                       new[]
                       {
                           new QueryParameter("name", measurementType.Name)
                       }
                   ) == 1;
        }

        public async Task<bool> UpdateMeasurementTypeAsync(MeasurementType measurementType) {
            return await _template.ExecuteAsync(
                       "UPDATE measurement_type SET name = @name WHERE id = @id",
                       new[]
                       {
                           new QueryParameter("name", measurementType.Name),
                           new QueryParameter("id", measurementType.Id),
                       }
                   ) == 1;
        }

        public async Task<bool> DeleteMeasurementTypeAsync(MeasurementType measurementType) {
            return await _template.ExecuteAsync(
                       "delete from measurement_type where id = @id",
                       new[] { new QueryParameter("@id", measurementType.Id) }
                   ) == 1;
        }
    }
}
