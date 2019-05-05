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
    public class ProvinceDaoTest {
        private static readonly string configName = "WetrDBConnection";

        [TestMethod]
        public async Task FindAll() {
            IProvinceDao provinceDao = new AdoProvinceDao(DefaultConnectionFactory.FromConfiguration(configName));

            IEnumerable<Province> provinces;

            provinces = await provinceDao.FindAllAsync();
            Assert.IsTrue(provinces.Any());
        }

        [TestMethod]
        public async Task FindById() {
            IProvinceDao provinceDao = new AdoProvinceDao(DefaultConnectionFactory.FromConfiguration(configName));

            Province province1 = await provinceDao.FindByIdAsync(1);
            Assert.AreEqual(province1.Id, 1);

            Province province5000 = await provinceDao.FindByIdAsync(5000);
            Assert.IsNull(province5000);
        }

        [TestMethod]
        public async Task FindByName() {
            IProvinceDao provinceDao = new AdoProvinceDao(DefaultConnectionFactory.FromConfiguration(configName));

            IEnumerable<Province> provinces;

            provinces = await provinceDao.FindByNameAsync("Oberösterreich");
            Assert.IsTrue(provinces.Any());

            provinces = await provinceDao.FindByNameAsync("Freistadt");
            Assert.IsFalse(provinces.Any());
        }

        [TestMethod]
        public async Task Add() {
            IProvinceDao provinceDao = new AdoProvinceDao(DefaultConnectionFactory.FromConfiguration(configName));

            Province province = new Province("Bayern");

            bool inserted = await provinceDao.AddProvinceAsync(province);
            Assert.IsTrue(inserted);

            province = (await provinceDao.FindByNameAsync("Bayern")).FirstOrDefault();

            await provinceDao.DeleteProvinceAsync(province);
        }

        [TestMethod]
        public async Task Update() {
            IProvinceDao provinceDao = new AdoProvinceDao(DefaultConnectionFactory.FromConfiguration(configName));

            Province province = await provinceDao.FindByIdAsync(1);

            string originalName = province.Name;
            province.Name = "New name";
            bool update1 = await provinceDao.UpdateProvinceAsync(province);
            Assert.IsTrue(update1);

            province = await provinceDao.FindByIdAsync(1);
            Assert.AreEqual(province.Name, "New name");

            province.Name = originalName;
            bool update2 = await provinceDao.UpdateProvinceAsync(province);
            Assert.IsTrue(update2);

            province = await provinceDao.FindByIdAsync(1);
            Assert.AreEqual(province.Name, originalName);
        }

        [TestMethod]
        public async Task Delete() {
            IProvinceDao provinceDao = new AdoProvinceDao(DefaultConnectionFactory.FromConfiguration(configName));

            Province province = new Province("Bayern");

            bool inserted = await provinceDao.AddProvinceAsync(province);
            Assert.IsTrue(inserted);

            province = (await provinceDao.FindByNameAsync("Bayern")).FirstOrDefault();
            bool deleted = await provinceDao.DeleteProvinceAsync(province);
            Assert.IsTrue(deleted);

            province = (await provinceDao.FindByNameAsync("Bayern")).FirstOrDefault();
            Assert.IsNull(province);
        }
    }
}
