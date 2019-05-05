using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Server.Implementation;
using Wetr.Server.Interface;

namespace Wetr.Server.Factory {
    public class CommunityManagerFactory {
        private static ICommunityManager _communityManager;

        public static ICommunityManager GetCommunityManager() {
            return _communityManager ?? (_communityManager = new CommunityManager());
        }
    }
}
