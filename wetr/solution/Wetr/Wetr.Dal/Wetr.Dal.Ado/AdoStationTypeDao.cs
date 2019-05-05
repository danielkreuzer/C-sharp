using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dal.Common;
using Wetr.Dal.Interface;
using Wetr.Domain;

namespace Wetr.Dal.Ado {
    public class AdoStationTypeDao : IStationTypeDao {

        private static readonly RowMapper<StationType> StationTypeMapper =
            row => new StationType {
                Id = (int)row["id"],
                Manufacturer = (string)row["manufacturer"],
                Model = (string)row["model"]
            };

        private readonly AdoTemplate _template;

        public AdoStationTypeDao(IConnectionFactory connectionFactory) {
            this._template = new AdoTemplate(connectionFactory);
        }

        public async Task<StationType> FindByIdAsync(int id) {
            return (await _template.QueryAsync("SELECT * FROM station_type WHERE id =@id",
                new[] { new QueryParameter("@id", id)}, StationTypeMapper)).FirstOrDefault();
        }

        public async Task<IEnumerable<StationType>> FindAllAsync() {
            return await _template.QueryAsync("SELECT * FROM station_type", StationTypeMapper);
        }

        public async Task<IEnumerable<StationType>> FindByManufacturerModelAsync(string manufacturer, string model) {
            return await _template.QueryAsync("SELECT * FROM station_type WHERE manufacturer =@manufacturer AND model =@model",
                new[] { new QueryParameter("@manufacturer", manufacturer), new QueryParameter("@model", model)  }, StationTypeMapper);
        }

        public async Task<IEnumerable<StationType>> FindByManufacturerAsync(string manufacturer) {
            return await _template.QueryAsync("SELECT * FROM station_type WHERE manufacturer =@manufacturer",
                new[] { new QueryParameter("@manufacturer", manufacturer) }, StationTypeMapper);
        }

        public async Task<bool> UpdateAllAsync(StationType stationType) {
            return await _template.ExecuteAsync(
                "UPDATE station_type SET manufacturer = @manufacturer, model = @model WHERE id = @id",
                new[] {
                    new QueryParameter("@manufacturer", stationType.Manufacturer),
                    new QueryParameter("@model", stationType.Model),
                    new QueryParameter("@id", stationType.Id)
                }) == 1;
        }

        public async Task<bool> AddStationTypeAsync(StationType stationType) {
            return await _template.ExecuteAsync(
                       "INSERT INTO station_type (manufacturer, model) " +
                       "VALUES (@manufacturer, @model)",
                       new[] {
                           new QueryParameter("@manufacturer", stationType.Manufacturer),
                           new QueryParameter("@model", stationType.Model)
                       }) == 1;
        }

        public async Task<bool> DeleteStationTypeAsync(StationType stationType) {
            return await _template.ExecuteAsync(
                       "DELETE FROM station_type WHERE id = @id",
                       new[] {
                           new QueryParameter("@id", stationType.Id)
                       }) == 1;
        }
    }
}
