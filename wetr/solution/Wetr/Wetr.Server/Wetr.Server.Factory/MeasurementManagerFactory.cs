using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Server.Implementation;
using Wetr.Server.Interface;

namespace Wetr.Server.Factory {
    public static class MeasurementManagerFactory {
        private static IMeasurementManager _MeasurementManager;

        public static IMeasurementManager GetMeasurementManager() {
            return _MeasurementManager ?? (_MeasurementManager = new MeasurementManager());
        }
    }
}
