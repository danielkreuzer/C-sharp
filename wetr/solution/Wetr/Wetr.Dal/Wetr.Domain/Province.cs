using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wetr.Domain {
    public class Province {

        public int Id { get; set; }
        public string Name { get; set; }

        public Province() {}

        public Province(int id, string name) {
            Id = id;
            Name = name;
        }

        public Province(string name) {
            Name = name;
        }
    }
}
