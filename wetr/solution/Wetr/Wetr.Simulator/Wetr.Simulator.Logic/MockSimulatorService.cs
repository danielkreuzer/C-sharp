using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Simulator.Logic {
    public class MockSimulatorService : ISimulatorService {
        private List<Measurement> _measurements = new List<Measurement>();

        public async Task<IEnumerable<Station>> GetStations() {
            ICollection<Station> stations = new List<Station> {
                new Station(1, "Station 1", 0, 40.000, 13.000, 12, 450, 0),
                new Station(2, "Station 2", 0, 40.500, 14.000, 12, 600, 0),
                new Station(3, "Station 3", 0, 41.020, 13.300, 12, 1200, 0),
            };

            return stations;
        }

        public async Task<IEnumerable<MeasurementType>> GetMeasurementTypes() {
            ICollection<MeasurementType> measurementTypes = new List<MeasurementType>() {
                new MeasurementType(1, "temperature"),
                new MeasurementType(2, "air_pressure"),
                new MeasurementType(3, "rainfall"),
                new MeasurementType(4, "humidity"),
                new MeasurementType(5, "wind_speed"),
                new MeasurementType(6, "wind_direction"),
            };

            return measurementTypes;
        }

        public void SaveMeasurement(Measurement measurement) {
            //_measurements.Add(measurement);
        }
    }
}
