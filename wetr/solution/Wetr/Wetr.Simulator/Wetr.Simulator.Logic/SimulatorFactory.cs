using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wetr.Simulator.Logic {
    public class SimulatorFactory {
        private static ISimulator _simulator;

        public static ISimulator GetSimulator() {
            return _simulator ?? (_simulator = new Simulator(new RestSimulatorService()));
        }
    }
}
