using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Dal.Interface {
    public interface IUnitDao {
        Task<IEnumerable<Unit>> FindAllAsync();
        Task<Unit> FindByIdAsync(int id);
        Task<IEnumerable<Unit>> FindByShortNameAsync(string name);
        Task<IEnumerable<Unit>> FindByLongNameAsync(string name);
        Task<bool> AddUnitAsync(Unit unit);
        Task<bool> UpdateUnitAsync(Unit unit);
        Task<bool> DeleteUnitAsync(Unit unit);
    }
}
