using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wetr.Domain {
    public class Measurement {

        public int Id { get; set; }
        public double Value { get; set; }
        public int TypeId { get; set; }
        public MeasurementType MeasurementType { get; set; }
        public DateTime Timestamp { get; set; }
        public int StationId { get; set; }
        public Station Station { get; set; }
        public int UnitId { get; set; }
        public Unit Unit { get; set; }

        public Measurement() {}

        public Measurement(int id, double value, int typeId, DateTime timestamp, int stationId, int unitId) {
            Id = id;
            Value = value;
            TypeId = typeId;
            Timestamp = timestamp;
            StationId = stationId;
            UnitId = unitId;
            MeasurementType = null;
            Unit = null;
        }

        public Measurement(double value, int typeId, DateTime timestamp, int stationId, int unitId) {
            Value = value;
            TypeId = typeId;
            Timestamp = timestamp;
            StationId = stationId;
            UnitId = unitId;
            MeasurementType = null;
            Unit = null;
        }

        public Measurement(int id, double value, MeasurementType measurementType, DateTime timestamp, Station station, Unit unit) {
            Id = id;
            Value = value;
            MeasurementType = measurementType;
            Timestamp = timestamp;
            Station = station;
            Unit = unit;
        }
    }
}
