using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Common {
    public delegate T RowMapper<T>(IDataRecord row);
}
