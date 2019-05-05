using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Server.Interface;
using Wetr.Server.Implementation;

namespace Wetr.Server.Factory {
    public static class UserManagerFactory {
        private static IUserManager _userManager;

        public static IUserManager GetUserManager() {
            return _userManager ?? (_userManager = new UserManager());
        }
    }
}
