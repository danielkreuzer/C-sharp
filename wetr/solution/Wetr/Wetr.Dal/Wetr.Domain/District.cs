using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wetr.Domain {
    public class District {

        public int Id { get; set; }
        public string Name { get; set; }
        public int ProvinceId { get; set; }

        public District() {}

        public District(int id, string name, int provinceId) {
            Id = id;
            Name = name;
            ProvinceId = provinceId;
        }

        public District(string name, int provinceId) {
            Name = name;
            ProvinceId = provinceId;
        }
    }
}
