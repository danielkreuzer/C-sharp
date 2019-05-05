using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Wetr.Simulator.Gui.Annotations;
using Wetr.Simulator.Logic;

namespace Wetr.Simulator.Gui.ViewModel {
    public class SimulatorVM : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public SimulatorVM(ISimulator simulator) {
            Tabs = new ObservableCollection<IViewModelBase>();

            Tabs.Add(new SettingsVM(simulator));

            simulator.StationIsInSimulationChanged += (added, station) => {
                if (added) {
                    Tabs.Add(new StationVM(station, simulator));
                } else {
                    for (int i = 0; i < Tabs.Count; i++) {
                        if (Tabs[i] is StationVM stationVm && stationVm.Id == station.Id) {
                            Tabs.RemoveAt(i);
                        }
                    }
                }
            };
        }
        
        public ObservableCollection<IViewModelBase> Tabs { get; set; }

    }
}
