using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Wetr.Domain;
using Wetr.Simulator.Gui.Annotations;
using Wetr.Simulator.Logic;

namespace Wetr.Simulator.Gui.ViewModel {
    public class SettingsVM : IViewModelBase {
        private readonly ISimulator _simulator;

        public SettingsVM(ISimulator simulator) {
            this._simulator = simulator;
            SynchronizationContext uiContext = SynchronizationContext.Current;

            // start threads for async operations with UI-Context
            Stations = new ObservableCollection<StationVM>();
            Task.Run(() => LoadStationsAsync(uiContext));

            MeasurementTypes = new ObservableCollection<MeasurementType>();
            Task.Run(() => LoadTypesAsync(uiContext));

            // register event handlers to watch for simulation status changes
            simulator.IsSimulatingChanged += value => IsSimulating = value;

            simulator.StationIsInSimulationChanged += (added, station) => {
                if (added) {
                    SelectedStations.Add(station);
                } else {
                    for (int i = 0; i < SelectedStations.Count; i++) {
                        if (SelectedStations[i].Id == station.Id) {
                            SelectedStations.RemoveAt(i);
                        }
                    }
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedStations)));
                UpdateSimulationReady();
            };
        }

        public string TabHeader => "Settings";

        public async void LoadStationsAsync(SynchronizationContext uiContext) {
            foreach (var station in await _simulator.GetStations()) {
                uiContext.Send(x => {
                    Stations.Add(new StationVM(station, _simulator));
                    _stationsLoaded = true;
                    UpdateSimulationReady();
                }, null);
            }
        }

        public async void LoadTypesAsync(SynchronizationContext uiContext) {
            foreach (var type in await _simulator.GetMeasurementTypes()) {
                uiContext.Send(x => {
                    MeasurementTypes.Add(type);
                    _typesLoaded = true;
                    UpdateSimulationReady();
                }, null);
            }
            uiContext.Send(x => CurrentMeasurementType = MeasurementTypes.FirstOrDefault(), null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<StationVM> Stations { get; set; }
        public ObservableCollection<MeasurementType> MeasurementTypes { get; set; }

        public MeasurementType CurrentMeasurementType {
            get => _simulator.CurrentMeasurementType ?? MeasurementTypes.FirstOrDefault();
            set {
                _simulator.CurrentMeasurementType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentMeasurementType)));
            }
        }

        public ObservableCollection<Station> SelectedStations {
            get {
                var collection = new ObservableCollection<Station>();
                _simulator.SelectedStations.ForEach(s => collection.Add(s));
                return collection;
            }
            set => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedStations)));
        }

        public Station CurrentStation {
            get => _simulator.CurrentStation;
            set {
                _simulator.CurrentStation = value;
                UpdateSimulationReady();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentStation)));
            }
        }

        public DateTime CurrentBeginDate {
            get => _simulator.Begin;
            set {
                _simulator.Begin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentBeginDate)));
            }
        }

        public DateTime CurrentEndDate {
            get => _simulator.End;
            set {
                _simulator.End = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentEndDate)));
            }
        }

        public bool CurrentLoadTestStatus {
            get => _simulator.IsLoadTesting;
            set {
                _simulator.IsLoadTesting = value;
                UpdateSimulationReady();
                if (value) {
                    CurrentStation = null;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLoadTestStatus)));
            }
        }

        public double CurrentSpeed {
            get => _simulator.Speed;
            set {
                _simulator.Speed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentSpeed)));
            }
        }

        public int CurrentInterval {
            get => _simulator.Interval;
            set {
                _simulator.Interval = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentInterval)));
            }
        }

        public string CurrentFromValue {
            get => _simulator.FromValue.ToString();
            set {
                // handle inputs in english number format
                // .NET only accepts system culture (German) format by default 
                value = value.Replace('.', ',');

                if (double.TryParse(value, out var newValue)) {
                    _simulator.FromValue = newValue;
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentFromValue)));
            }
        }

        public string CurrentToValue {
            get => _simulator.ToValue.ToString();
            set {
                value = value.Replace('.', ',');

                if (double.TryParse(value, out var newValue)) {
                    _simulator.ToValue = newValue;
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentToValue)));
            }
        }

        public IEnumerable<DistributionStrategy> DistributionStrategies {
            get => Enum.GetValues(typeof(DistributionStrategy)).Cast<DistributionStrategy>();
        }

        public DistributionStrategy CurrentDistributionStrategy {
            get => _simulator.Strategy;
            set {
                _simulator.Strategy = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentDistributionStrategy)));
            }
        }

        public bool IsSimulating {
            get => _simulator.IsSimulating;
            private set => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSimulating)));
        }

        private bool _stationsLoaded;
        private bool _typesLoaded;

        private bool _simulationReady;

        public bool SimulationReady {
            get => _simulationReady;
            set {
                _simulationReady = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SimulationReady)));
            }
        }

        private void UpdateSimulationReady() {
            SimulationReady = _stationsLoaded && _typesLoaded && (CurrentStation != null || CurrentLoadTestStatus) && SelectedStations.Count > 0;
        }

        private ICommand _startCommand;

        public ICommand StartCommand {
            get => _startCommand ?? (_startCommand = new RelayCommand(param => _simulator.StartSimulator()));
        }

        private ICommand _stopCommand;

        public ICommand StopCommand {
            get => _stopCommand ?? (_stopCommand = new RelayCommand(param => _simulator.StopSimulator()));
        }
    }
}
