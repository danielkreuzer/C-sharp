using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wetr.Domain {
    public class DoubleCallback {
        public double Value { get; set; }

        public DoubleCallback(double value) {
            Value = value;
        }

        public DoubleCallback() {
            
        }
    }
}
