using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Simulator.Logic {
    public interface ISimulatorService {
        Task<IEnumerable<Station>> GetStations();
        Task<IEnumerable<MeasurementType>> GetMeasurementTypes();
        void SaveMeasurement(Measurement measurement);
    }
}
