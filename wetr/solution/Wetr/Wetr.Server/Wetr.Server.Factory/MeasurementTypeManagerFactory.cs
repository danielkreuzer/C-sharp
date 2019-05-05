using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Server.Implementation;
using Wetr.Server.Interface;

namespace Wetr.Server.Factory {
    public class MeasurementTypeManagerFactory {
        private static IMeasurementTypeManager _measurementTypeManager;

        public static IMeasurementTypeManager GetMeasurementTypeManager() {
            return _measurementTypeManager ?? (_measurementTypeManager = new MeasurementTypeManager());
        }
    }
}
