using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Simulator.Logic {
    public interface ISimulator {
        Task<IEnumerable<Station>> GetStations();
        Task<IEnumerable<MeasurementType>> GetMeasurementTypes();

        void AddStation(Station station);
        void RemoveStation(Station station);

        List<Station> SelectedStations { get; }
        Station CurrentStation { get; set; }
        bool StationIsInSimulation(Station station);

        /// <summary>
        /// This event triggers each time, the collection of selected stations changes
        /// The first parameter (bool) says whether a station was added (true) or removed (false)
        /// The second parameter (Station) is the affected station
        /// </summary>
        event Action<bool, Station> StationIsInSimulationChanged;

        MeasurementType CurrentMeasurementType { get; set; }

        DateTime Begin { get; set; }
        DateTime End { get; set; }

        bool IsLoadTesting { get; set; }

        double Speed { get; set; }
        int Interval { get; set; }

        double FromValue { get; set; }
        double ToValue { get; set; }

        DistributionStrategy Strategy { get; set; }

        bool IsSimulating { get; }
        event Action<bool> IsSimulatingChanged;

        event Action<int, MeasurementType, Measurement> MeasurementGenerated;

        void StartSimulator();
        void StopSimulator();
    }
}
