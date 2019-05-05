using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wetr.Simulator.Gui {
    public interface IViewModelBase : INotifyPropertyChanged {
        string TabHeader { get; }
    }
}
