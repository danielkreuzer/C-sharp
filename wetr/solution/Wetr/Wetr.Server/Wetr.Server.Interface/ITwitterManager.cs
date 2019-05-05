using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Server.Interface {
    public interface ITwitterManager {
        Task SendAlarmTweet(Measurement measurement);
    }
}
