using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Wetr.Domain;
using Wetr.Server.Factory;
using Wetr.Server.Interface;

namespace Wetr.WebService.REST.Controllers {

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    public class UsersController : ApiController {
        public class LoginDetails {
            public string Username;
            public string Password;
        }

        public class OAuthLoginDetails {
            public string email;
            public string family_name;
            public string given_name;
        }

        private readonly IUserManager _userManager = UserManagerFactory.GetUserManager();

        [HttpGet]
        [Route("users")]
        public async Task<User> GetUserByEmail([FromUri] string email) {
            try {
                return await _userManager.GetUserByEmail(email);
            }
            catch (Exception) {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        [HttpGet]
        [Route("users/id")]
        public async Task<User> GetUserById([FromUri] int id) {
            try {
                return await _userManager.GetUserById(id);
            }
            catch (Exception) {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("users/login")]
        public async Task<User> CheckLogin([FromBody] LoginDetails loginDetails) {
            try {
                if (await _userManager.CheckLogin(loginDetails.Username, loginDetails.Password)) {
                    return await _userManager.GetUserByUsername(loginDetails.Username);
                } else {
                    return null;
                }
            }
            catch (Exception) {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        [HttpPost]
        [Route("users/login/oauth")]
        public async Task<User> CheckOAuthLogin([FromBody] OAuthLoginDetails loginDetails) {
            try {
                var user = await _userManager.GetUserByEmail(loginDetails.email);
                if (user != null) {
                    return user;
                } else {
                    await _userManager.AddUser(new User(loginDetails.email, "password", loginDetails.email,
                        loginDetails.given_name, loginDetails.family_name, new DateTime(1985, 2, 3), 1));

                    return await _userManager.GetUserByEmail(loginDetails.email);
                }
            }
            catch (Exception) {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }
    }
}