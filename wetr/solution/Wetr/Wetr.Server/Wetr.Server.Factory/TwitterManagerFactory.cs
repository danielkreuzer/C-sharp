using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Server.Implementation;
using Wetr.Server.Interface;

namespace Wetr.Server.Factory {
    public static class TwitterManagerFactory {
        private static ITwitterManager _twitterManager;

        public static ITwitterManager GetTwitterManager() {
            return _twitterManager ?? (_twitterManager = new TwitterManager());
        }
    }
}
