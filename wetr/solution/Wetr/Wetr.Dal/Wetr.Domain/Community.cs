using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wetr.Domain {
    public class Community {

        public int Id { get; set; }
        public int ZipCode { get; set; }
        public string Name { get; set; }
        public int DistrictId { get; set; }
        public string ZipName { get; set; }

        public Community() {}

        public Community(int id, int zipCode, string name, int districtId) {
            Id = id;
            ZipCode = zipCode;
            Name = name;
            DistrictId = districtId;
            ZipName = ZipCode + " " + Name;
        }

        public Community(int zipCode, string name, int districtId) {
            ZipCode = zipCode;
            Name = name;
            DistrictId = districtId;
            ZipName = ZipCode + " " + Name;
        }
    }
}
