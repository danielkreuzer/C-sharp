using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wetr.Domain;
using Wetr.Server.Interface;

namespace Wetr.Server.Test {
    [TestClass]
    public class UserManagerTest {

        private static IUserManager GetMockUserManager() {
            var mock = new Mock<IUserManager>();
             
            mock.Setup(m => m.CheckLogin("username", "password")).ReturnsAsync((string username, string password) => true);
            mock.Setup(m => m.CheckLogin("username", "wrong_password")).ReturnsAsync((string username, string password) => false);

            mock.Setup(m => m.GetUserByUsername("username")).ReturnsAsync((string username) => new User());
            mock.Setup(m => m.GetUserByUsername("wrong_username")).ReturnsAsync((string username) => null);

            mock.Setup(m => m.GetUserByEmail("email")).ReturnsAsync((string email) => new User());
            mock.Setup(m => m.GetUserByEmail("wrong_email")).ReturnsAsync((string email) => null);

            mock.Setup(m => m.UpdateUser(It.IsNotNull<User>())).ReturnsAsync((User user) => true);
            mock.Setup(m => m.UpdateUser(null)).ReturnsAsync((User user) => false);

            mock.Setup(m => m.UpdatePassword("username", "password")).ReturnsAsync((string username, string password) => true);
            mock.Setup(m => m.UpdatePassword("wrong_username", "password")).ReturnsAsync((string username, string password) => false);

            mock.Setup(m => m.AddUser(It.IsNotNull<User>())).ReturnsAsync((User user) => true);
            mock.Setup(m => m.AddUser(null)).ReturnsAsync((User user) => false);

            mock.Setup(m => m.DeleteUser(It.IsNotNull<User>())).ReturnsAsync((User user) => true);
            mock.Setup(m => m.DeleteUser(null)).ReturnsAsync((User user) => false);

            return mock.Object;
        }

        [TestMethod]
        public async Task CheckLogin() {
            IUserManager userManager = GetMockUserManager();

            Assert.IsTrue(await userManager.CheckLogin("username", "password"));

            Assert.IsFalse(await userManager.CheckLogin("username", "wrong password"));
        }

        [TestMethod]
        public async Task GetUserByUsername() {
            IUserManager userManager = GetMockUserManager();

            Assert.IsNotNull(await userManager.GetUserByUsername("username"));

            Assert.IsNull(await userManager.GetUserByUsername("wrong username"));
        }

        [TestMethod]
        public async Task GetUserByEmail() {
            IUserManager userManager = GetMockUserManager();

            Assert.IsNotNull(await userManager.GetUserByEmail("email"));

            Assert.IsNull(await userManager.GetUserByEmail("wrong_email"));
        }

        [TestMethod]
        public async Task UpdateUser() {
            IUserManager userManager = GetMockUserManager();

            Assert.IsTrue(await userManager.UpdateUser(new User()));

            Assert.IsFalse(await userManager.UpdateUser(null));
        }

        [TestMethod]
        public async Task UpdatePassword() {
            IUserManager userManager = GetMockUserManager();

            Assert.IsTrue(await userManager.UpdatePassword("username", "password"));

            Assert.IsFalse(await userManager.UpdatePassword("wrong username", "password"));
        }

        [TestMethod]
        public async Task AddUser() {
            IUserManager userManager = GetMockUserManager();

            Assert.IsTrue(await userManager.AddUser(new User()));

            Assert.IsFalse(await userManager.AddUser(null));
        }

        [TestMethod]
        public async Task DeleteUser() {
            IUserManager userManager = GetMockUserManager();

            Assert.IsTrue(await userManager.DeleteUser(new User()));

            Assert.IsFalse(await userManager.DeleteUser(null));
        }
    }
}
