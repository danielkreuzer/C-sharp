using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wetr.Domain {
    public class MeasurementType {
        public int Id { get; set; }
        public string Name { get; set; }

        public MeasurementType() {
        }

        public MeasurementType(int id, string name) {
            Id = id;
            this.Name = name;
        }

        public MeasurementType(string name) {
            this.Name = name;
        }
    }
}
