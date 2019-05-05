using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wetr.Domain {
    public class Unit {

        public int Id { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }

        public Unit() {}

        public Unit(int id, string shortName, string longName) {
            Id = id;
            ShortName = shortName;
            LongName = longName;
        }

        public Unit(string shortName, string longName) {
            ShortName = shortName;
            LongName = longName;
        }
    }
}
