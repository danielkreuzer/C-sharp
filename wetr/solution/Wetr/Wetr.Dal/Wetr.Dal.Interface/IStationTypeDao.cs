using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Dal.Interface {
    public interface IStationTypeDao {
        Task<StationType> FindByIdAsync(int id);
        Task<IEnumerable<StationType>> FindAllAsync();
        Task<IEnumerable<StationType>> FindByManufacturerModelAsync(string manufacturer, string model);
        Task<IEnumerable<StationType>> FindByManufacturerAsync(string manufacturer);
        Task<bool> UpdateAllAsync(StationType stationType);
        Task<bool> AddStationTypeAsync(StationType stationType);
        Task<bool> DeleteStationTypeAsync(StationType stationType);
    }
}
