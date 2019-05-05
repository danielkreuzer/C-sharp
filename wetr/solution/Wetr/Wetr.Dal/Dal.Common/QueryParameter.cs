using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Common {
    public class QueryParameter {
        public QueryParameter(string name, object value) {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; }
        public object Value { get; }
    }
}
