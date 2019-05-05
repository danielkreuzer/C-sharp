using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Dal.Interface {
    public interface IMeasurementTypeDao {
        Task<IEnumerable<MeasurementType>> FindAllAsync();
        Task<MeasurementType> FindByIdAsync(int id);
        Task<MeasurementType> FindByNameAsync(string name);
        Task<bool> AddMeasurementTypeAsync(MeasurementType measurementType);
        Task<bool> UpdateMeasurementTypeAsync(MeasurementType measurementType);
        Task<bool> DeleteMeasurementTypeAsync(MeasurementType measurementType);
    }
}
