using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wetr.Domain;

namespace Wetr.Simulator.Logic {
    public class Simulator : ISimulator {
        private readonly ISimulatorService _simulatorService;

        public Simulator(ISimulatorService simulatorService) {
            _simulatorService = simulatorService;

            var yesterday = DateTime.Now.Subtract(TimeSpan.FromDays(1));
            var today = DateTime.Now;

            Begin = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day);
            End = new DateTime(today.Year, today.Month, today.Day);
        }

        public async Task<IEnumerable<Station>> GetStations() {
            return await _simulatorService.GetStations();
        }

        public async Task<IEnumerable<MeasurementType>> GetMeasurementTypes() {
            return await _simulatorService.GetMeasurementTypes();
        }

        public List<Station> SelectedStations { get; } = new List<Station>();

        public Station CurrentStation { get; set; }

        public event Action<bool, Station> StationIsInSimulationChanged;

        public void AddStation(Station station) {
            if (!StationIsInSimulation(station)) {
                SelectedStations.Add(station);
                StationIsInSimulationChanged?.Invoke(true, station);
            }
        }

        public void RemoveStation(Station station) {
            SelectedStations.RemoveAll(s => s.Id == station.Id);
            StationIsInSimulationChanged?.Invoke(false, station);
        }

        public bool StationIsInSimulation(Station station) {
            return SelectedStations.FindAll(s => s.Id == station.Id).Any();
        }

        public MeasurementType CurrentMeasurementType { get; set; }

        public DateTime Begin { get; set; }
        public DateTime End { get; set; }

        public bool IsLoadTesting { get; set; } = false;

        public double Speed { get; set; } = 1;
        public int Interval { get; set; } = 10;

        public double FromValue { get; set; } = 0;
        public double ToValue { get; set; } = 10;

        public DistributionStrategy Strategy { get; set; } = DistributionStrategy.Random;


        public bool IsSimulating { get; private set; }

        public event Action<bool> IsSimulatingChanged;

        public event Action<int, MeasurementType, Measurement> MeasurementGenerated;


        private CancellationTokenSource _tokenSource;
        private int _runningTasks = 0;

        public void StartSimulator() {
            IsSimulating = true;
            IsSimulatingChanged?.Invoke(IsSimulating);

            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;

            if (IsLoadTesting) {
                SelectedStations.ForEach(station => {
                    Task.Run(() => Simulate(station, token), token);
                    _runningTasks++;
                });

            } else {
                Task.Run(() => Simulate(CurrentStation, token), token);
                _runningTasks++;
            }
        }

        private readonly object _lockObject = new object();

        private void TaskCompleted() {
            lock (_lockObject) {
                _runningTasks--;
            }

            if (_runningTasks == 0) {
                StopSimulator();
            }
        }

        public void StopSimulator() {
            _tokenSource?.Cancel();

            IsSimulating = false;
            IsSimulatingChanged?.Invoke(IsSimulating);
        }

        private void Simulate(Station station, CancellationToken token) {
            double lastValue = double.NaN;

            // for smart strategy:
            double numberOfValuesGenerated = 0;

            var date = Begin;

            while (date < End && !token.IsCancellationRequested) {
                SetLinearValues();

                double value = 0;

                switch (Strategy) {
                    case DistributionStrategy.Linear:
                        value = GetLinearValue(lastValue);
                        break;
                    case DistributionStrategy.Random:
                        value = GetRandomValue(lastValue);
                        break;
                    case DistributionStrategy.Smart:
                        value = GetSmartValue(ref numberOfValuesGenerated);
                        break;
                }

                lastValue = value;
                value = Math.Round(value, 2);

                var measurement = new Measurement(value, CurrentMeasurementType.Id, date, station.Id, CurrentMeasurementType.Id);

                if (value >= FromValue && value <= ToValue) {
                    if (!IsLoadTesting) {
                        MeasurementGenerated?.Invoke(station.Id, CurrentMeasurementType, measurement);
                    }

                    _simulatorService.SaveMeasurement(measurement);
                }
                
                Thread.Sleep((int)Math.Floor((Interval / Speed) * 1000));

                date = date.AddMinutes(Speed);
            }

            TaskCompleted();
        }

        private double _numberOfValuesToGenerate;
        private double _areaToCover;
        private double _difference;

        private void SetLinearValues() {
            _numberOfValuesToGenerate = Math.Floor((End - Begin).TotalMinutes / (Speed));
            _areaToCover = Math.Abs(ToValue - FromValue);
            _difference = _areaToCover / _numberOfValuesToGenerate;
        }

        private double GetLinearValue(double lastValue) {
            if (double.IsNaN(lastValue)) {
                lastValue = FromValue;
            }

            double result;

            if (FromValue < ToValue) {
                result = lastValue + _difference;
            } else {
                result = lastValue - _difference;
            }

            return result;
        }

        private readonly Random _random = new Random();

        private double GetRandomValue(double lastValue) {
            double min, max;

            // base min and max of lastValue if lastValue is valid
            if (!double.IsNaN(lastValue) && lastValue > FromValue && lastValue < ToValue) {
                // vary by 10%
                var variation = lastValue * 0.1;
                min = lastValue - variation;
                max = lastValue + variation;

            } else {
                min = FromValue;
                max = ToValue;
            }

            double result = _random.NextDouble() * (max - min) + min;
            return Math.Abs(result) > 0.05 ? result : 0.1;
        }

        private double GetSmartValue(ref double numberOfValuesGenerated) {
            var average = (FromValue + ToValue) / 2;

            var stepsPerDay = Math.Floor(TimeSpan.FromDays(1).TotalMinutes / (Speed));
            var stepSize = _areaToCover / stepsPerDay;

            // reset counter at end of day
            if (numberOfValuesGenerated > stepsPerDay) {
                numberOfValuesGenerated = 0;
            }

            var xAxisPos = FromValue + (stepSize * numberOfValuesGenerated);

            var size = GetSize(Math.Abs(average));
            var offset = GetValueOnBellCurve(xAxisPos, average, 1) * size * (size == 1 ? 10 : size);

            numberOfValuesGenerated++;
            return average + offset;
        }

        private static double GetValueOnBellCurve(double x, double mu, double sigma) {
            // https://www.thoughtco.com/normal-distribution-bell-curve-formula-3126278
            // https://www.wolframalpha.com/input/?i=(1%2F+(1+*+sqrt(2*pi)))*(e%5E(-(x-5)%5E2%2F(2*(1%5E2))))
            // https://www.wolframalpha.com/input/?i=NormalDistribution%5B5,+0.1%5D

            var part1 = 1 / (sigma * Math.Sqrt(2 * Math.PI));
            var exponent = Math.Pow(x - mu, 2) / (2 * Math.Pow(sigma, 2));

            return part1 * Math.Pow(Math.E, -exponent);
        }

        private static int GetSize(double value) {
            if (value < 10) return 1;
            if (value < 100) return 10;
            if (value < 1000) return 100;
            if (value < 10000) return 1000;
            if (value < 100000) return 10000;
            return 1;
        }
    }
}
