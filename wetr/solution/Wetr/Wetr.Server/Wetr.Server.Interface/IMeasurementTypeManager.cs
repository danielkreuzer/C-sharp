using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Server.Interface {
    public interface IMeasurementTypeManager {
        Task<IEnumerable<MeasurementType>> GetAllMeasurementTypes();
        Task<MeasurementType> FindMeasurementTypesById(int id);
        Task<MeasurementType> FindMeasurementTypesByName(string name);
    }
}
