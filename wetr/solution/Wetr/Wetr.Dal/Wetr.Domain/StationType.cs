using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wetr.Domain {
    public class StationType {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }

        public StationType() {}

        public StationType(int id, string manufacturer, string model) {
            Id = id;
            Manufacturer = manufacturer;
            Model = model;
        }

        public StationType(string manufacturer, string model) {
            Manufacturer = manufacturer;
            Model = model;
        }
    }
}
