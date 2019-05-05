using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Simulator.Logic {
    public class RestSimulatorService : ISimulatorService {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _baseUrl = "http://localhost:55233/api";

        public async Task<IEnumerable<Station>> GetStations() {
            var response = await _client.GetAsync(_baseUrl + "/stations");

            IEnumerable<Station> result = new List<Station>();
            if (response.IsSuccessStatusCode) {
                result = await response.Content.ReadAsAsync<IEnumerable<Station>>();
            }

            return result;
        }

        public async Task<IEnumerable<MeasurementType>> GetMeasurementTypes() {
            var response = await _client.GetAsync(_baseUrl + "/measurementtypes");

            IEnumerable<MeasurementType> result = new List<MeasurementType>();
            if (response.IsSuccessStatusCode) {
                result = await response.Content.ReadAsAsync<IEnumerable<MeasurementType>>();
            }

            return result;
        }

        public async void SaveMeasurement(Measurement measurement) {
            await _client.PostAsJsonAsync(_baseUrl + "/measurements", measurement);
        }
    }
}
