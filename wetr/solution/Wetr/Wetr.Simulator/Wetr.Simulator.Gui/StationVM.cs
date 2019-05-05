using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Wetr.Domain;
using Wetr.Simulator.Gui.Annotations;
using Wetr.Simulator.Gui.ViewModel;
using Wetr.Simulator.Logic;

namespace Wetr.Simulator.Gui.ViewModel {
    public class ValuesListEntry : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _measurementType;
        private double _value;
        private DateTime _date;

        public ValuesListEntry(MeasurementType measurementType, Measurement measurement) {
            MeasurementType = measurementType.Name;
            Value = measurement.Value;
            Date = measurement.Timestamp;
        }

        public string MeasurementType {
            get => _measurementType;
            set {
                _measurementType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MeasurementType)));
            }
        }

        public double Value {
            get => _value;
            set {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public DateTime Date {
            get => _date;
            set {
                _date = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Date)));
            }
        }
    }

    public class StationVM : IViewModelBase {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ISimulator _simulator;

        public ObservableCollection<ValuesListEntry> Values { get; } = new ObservableCollection<ValuesListEntry>();


        #region chart

        public ChartValues<ValuesListEntry> ChartValues { get; } = new ChartValues<ValuesListEntry>();

        private double _axisMax;
        private double _axisMin;
        private MeasurementType _lastMeasurementType;

        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }

        public double AxisMax {
            get => _axisMax;
            set {
                _axisMax = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AxisMax)));
            }
        }
        public double AxisMin {
            get => _axisMin;
            set {
                _axisMin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AxisMin)));
            }
        }

        public MeasurementType LastMeasurementType {
            get => _lastMeasurementType;
            set {
                _lastMeasurementType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastMeasurementType)));
            }
        }

        private void SetAxisLimits(DateTime now) {
            AxisMax = now.Ticks + TimeSpan.FromMinutes(60).Ticks; // force the axis to be 60 minutes ahead
            AxisMin = now.Ticks - TimeSpan.FromMinutes(60).Ticks; // and 60 minutes behind
        }

        #endregion


        public StationVM(Station station, ISimulator simulator) {
            this.Station = station;
            this._simulator = simulator;

            _simulator.StationIsInSimulationChanged += (added, s) => IsInSimulation = added;

            SynchronizationContext context = SynchronizationContext.Current;


            // initialize chart

            var mapper = Mappers.Xy<ValuesListEntry>()
                .X(m => m.Date.Ticks)
                .Y(m => m.Value);

            Charting.For<ValuesListEntry>(mapper);

            DateTimeFormatter = value => new DateTime((long)value).ToString("HH:mm");

            // AxisStep forces the distance between each separator in the X axis
            AxisStep = TimeSpan.FromMinutes(30).Ticks;

            // AxisUnit forces lets the axis know that we are plotting minutes
            // this is not always necessary, but it can prevent wrong labeling
            AxisUnit = TimeSpan.TicksPerMinute;

            SetAxisLimits(DateTime.Now);


            _simulator.MeasurementGenerated += (stationId, type, measurement) => {
                // reset chart if measurement type changes
                if (LastMeasurementType == null || LastMeasurementType.Id != type.Id) {
                    LastMeasurementType = type;
                    ChartValues.Clear();
                }

                if (stationId == Station.Id) {
                    context.Send(x => {
                        var entry = new ValuesListEntry(type, measurement);

                        Values.Insert(0, entry);
                        ChartValues.Add(entry);
                        SetAxisLimits(measurement.Timestamp);
                    }, null);
                }
            };
        }


        public string TabHeader => Name;

        public int Id => Station.Id;
        public string Name => Station.Name;

        public Station Station { get; }

        public bool IsInSimulation {
            get => _simulator.StationIsInSimulation(Station);
            private set => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInSimulation)));
        }

        private ICommand _addStationCommand;

        public ICommand AddStationCommand {
            get => _addStationCommand ??
                   (_addStationCommand = new RelayCommand(param => _simulator.AddStation(Station)));
        }

        private ICommand _removeStationCommand;

        public ICommand RemoveStationCommand {
            get => _removeStationCommand ??
                   (_removeStationCommand = new RelayCommand(param => _simulator.RemoveStation(Station)));
        }

        private ICommand _clearValuesCommand;

        public ICommand ClearValuesCommand {
            get => _clearValuesCommand ?? (_clearValuesCommand = new RelayCommand(param => {
                Values.Clear();
                ChartValues.Clear();
            }));
        }
    }
}
