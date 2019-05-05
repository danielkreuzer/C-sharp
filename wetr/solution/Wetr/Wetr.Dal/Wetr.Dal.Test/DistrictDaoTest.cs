using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dal.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wetr.Dal.Ado;
using Wetr.Dal.Interface;
using Wetr.Domain;

namespace Wetr.Test {
    [TestClass]
    public class DistrictDaoTest {
        private static readonly string configName = "WetrDBConnection";

        [TestMethod]
        public async Task TestFindAll() {
            IDistrictDao districtDao = new AdoDistrictDao(DefaultConnectionFactory.FromConfiguration(configName));

            IEnumerable<District> districts;

            districts = await districtDao.FindAllAsync();
            Assert.IsTrue(districts.Any());
        }

        [TestMethod]
        public async Task TestFindById() {
            IDistrictDao districtDao = new AdoDistrictDao(DefaultConnectionFactory.FromConfiguration(configName));

            District district1 = await districtDao.FindByIdAsync(101);
            Assert.AreEqual(district1.Id, 101);

            District district5000 = await districtDao.FindByIdAsync(5000);
            Assert.IsNull(district5000);
        }

        [TestMethod]
        public async Task TestFindByName() {
            IDistrictDao districtDao = new AdoDistrictDao(DefaultConnectionFactory.FromConfiguration(configName));

            IEnumerable<District> districts;

            districts = await districtDao.FindByNameAsync("Rohrbach");
            Assert.IsTrue(districts.Any());

            districts = await districtDao.FindByNameAsync("aaa");
            Assert.IsFalse(districts.Any());
        }

        [TestMethod]
        public async Task TestFindByDistrictId() {
            IDistrictDao districtDao = new AdoDistrictDao(DefaultConnectionFactory.FromConfiguration(configName));

            IEnumerable<District> districts;

            districts = await districtDao.FindByProvinceIdAsync(1);
            Assert.IsTrue(districts.Any());

            districts = await districtDao.FindByProvinceIdAsync(0);
            Assert.IsFalse(districts.Any());
        }

        [TestMethod]
        public async Task Add() {
            IDistrictDao districtDao = new AdoDistrictDao(DefaultConnectionFactory.FromConfiguration(configName));

            District district = new District("District", 1);

            bool inserted = await districtDao.AddDistrictAsync(district);
            Assert.IsTrue(inserted);

            district = (await districtDao.FindByNameAsync("District")).FirstOrDefault();

            await districtDao.DeleteDistrictAsync(district);
        }
        
        [TestMethod]
        public async Task Update() {
            IDistrictDao districtDao = new AdoDistrictDao(DefaultConnectionFactory.FromConfiguration(configName));

            District district = await districtDao.FindByIdAsync(101);

            string originalName = district.Name;
            district.Name = "New name";
            bool update1 = await districtDao.UpdateDistrictAsync(district);
            Assert.IsTrue(update1);

            district = await districtDao.FindByIdAsync(101);
            Assert.AreEqual(district.Name, "New name");

            district.Name = originalName;
            bool update2 = await districtDao.UpdateDistrictAsync(district);
            Assert.IsTrue(update2);

            district = await districtDao.FindByIdAsync(101);
            Assert.AreEqual(district.Name, originalName);
        }
        
        [TestMethod]
        public async Task Delete() {
            IDistrictDao districtDao = new AdoDistrictDao(DefaultConnectionFactory.FromConfiguration(configName));

            District district = new District("District", 1);

            bool inserted = await districtDao.AddDistrictAsync(district);
            Assert.IsTrue(inserted);

            district = (await districtDao.FindByNameAsync("District")).FirstOrDefault();
            bool deleted = await districtDao.DeleteDistrictAsync(district);
            Assert.IsTrue(deleted);

            district = (await districtDao.FindByNameAsync("District")).FirstOrDefault();
            Assert.IsNull(district);
        }
        
    }
}
