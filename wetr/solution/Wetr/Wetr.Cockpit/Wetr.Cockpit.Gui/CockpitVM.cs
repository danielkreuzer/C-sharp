using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Wetr.Domain;
using Wetr.Server.Factory;
using Wetr.Server.Interface;

namespace Wetr.Cockpit.Gui {
    public class CockpitVM : INotifyPropertyChanged {

        #region Helper Classes

        public delegate void SearchTypeFunction();

        public class SearchType {
            public string Name { get; set; }
            public SearchTypeFunction Func { get; set; }

            public SearchType(string name, SearchTypeFunction func) {
                Name = name;
                Func = func;
            }
        }

        public class ModeType {
            public string Name { get; set; }
            public int Mode { get; set; }

            public ModeType(string name, int mode) {
                Name = name;
                Mode = mode;
            }
        }

        #endregion

        #region Private Vars

        #region Context

        private Window _currentWindow;
        SynchronizationContext _context;
        private IStationDataManager _stationManager;
        private IUserManager _userManager;
        private IMeasurementManager _measurementManager;
        private User _currentUser;

        #endregion

        #region Lists / Containers

        private List<Station> _allStations = new List<Station>();
        private List<Community> _allCommunities = new List<Community>();
        private List<District> _allDistricts = new List<District>();
        private List<Province> _allProvinces = new List<Province>();
        private List<StationType> _allStationTypes = new List<StationType>();

        #endregion

        #region Visibilitys

        private bool _stationSettingsVisibility = false;
        private bool _addStationsVisibility = false;
        private bool _loginButtonVisibility = true;
        private bool _logoutButtonVisibility = false;
        private bool _analysisViewVisibility = true;
        private bool _raduisVisibility = false;
        private bool _stationSettingsButtonVisibility = false;
        private bool _selectedCommunityVisibilty = false;
        private bool _selectedDistrictVisibility = false;
        private bool _selectedProvinceVisibility = false;
        private bool _searchBoxVisibility = true;
        private bool _stationManagerVisibilty = false;

        #endregion

        #region Inputs

        private string _searchWord = "";
        private string _searchWordStation = "";
        private double _radius = 0;
        private double _searchLat = 0;
        private double _serachLong = 0;
        private DateTime _startDate = DateTime.Now;
        private DateTime _endDate = DateTime.Now;
        private int _groupByMode = 0;
        private int _calcMode = 0;
        private int _cummulationMode = 0;

        #endregion

        #region Selected

        private Station _currentStation;
        private SearchType _selectedSearchType;
        private Community _selectedCommunity;
        private District _selectedDistrict;
        private Province _selectedProvince;

        #endregion

        #region Message Texts

        private string _stationDeleteErrorText = "";
        private string _stationSettingsSuccessText = "";
        private string _loginText = "Please, log in!";
        private string _publicUpperErrorText = "";

        #endregion

        #endregion    

        #region Constructors

        public CockpitVM(IStationDataManager stationManager, IUserManager userManager, IMeasurementManager measurementManager, User currentUser, Window currentWindow) {
            _context = SynchronizationContext.Current;
            this._measurementManager = measurementManager;
            this.SearchTypes = new ObservableCollection<SearchType>();
            this._currentWindow = currentWindow;
            this._stationManager = stationManager;
            this._userManager = userManager;
            this._currentUser = currentUser;


            InitWithUser();
        }

        public CockpitVM(IStationDataManager stationDataManager, IUserManager userManager, IMeasurementManager measurementManager, Window currentWindow) {
            _context = SynchronizationContext.Current;
            this._measurementManager = measurementManager;
            this._currentWindow = currentWindow;
            this._stationManager = stationDataManager;
            this._userManager = userManager;
            this._currentUser = null;
            InitWithoutUser();
        }

        private void InitWithUser() {
            if (_currentUser == null) {
                InitWithUser();
            } else {
                _context = SynchronizationContext.Current;
                InitWithoutUser();
                Task.Run(() => LoadStations());
                _loginButtonVisibility = false;
                _logoutButtonVisibility = true;
                _loginText = $"Hello, {_currentUser.FirstName}";
                _stationSettingsButtonVisibility = true;
            }
        }

        private void InitWithoutUser() {
            _context = SynchronizationContext.Current;
            GraphInit();
            Task.Run(() => LoadCommunities());
            Task.Run(() => LoadTypes());
            Task.Run(() => LoadSearchTypes());
            Task.Run(() => LoadModes());
            Task.Run(() => LoadAllStations());
        }


        #endregion

        #region BindingProps

        #region Observable Collections

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Station> Stations { get; set; } = new ObservableCollection<Station>();
        public ObservableCollection<Station> SelectedStations { get; set; } = new ObservableCollection<Station>();
        public ObservableCollection<Station> SearchListStations { get; set; } = new ObservableCollection<Station>();
        public ObservableCollection<SearchType> SearchTypes { get; set; } = new ObservableCollection<SearchType>();
        public ObservableCollection<ModeType> GroupByModes { get; set; } = new ObservableCollection<ModeType>();
        public ObservableCollection<ModeType> CalcModes { get; set; } = new ObservableCollection<ModeType>();
        public ObservableCollection<ModeType> CommulationModes { get; set; } = new ObservableCollection<ModeType>();
        public ObservableCollection<StationType> StationTypes { get; set; } = new ObservableCollection<StationType>();
        public ObservableCollection<Community> Communities { get; set; } = new ObservableCollection<Community>();
        public ObservableCollection<Province> Provinces { get; set; } = new ObservableCollection<Province>();
        public ObservableCollection<District> Districts { get; set; } = new ObservableCollection<District>();



        //public ObservableCollection<StationType> StationTypes {
        //    get {
        //        var collection = new ObservableCollection<StationType>();
        //        _allStationTypes.ForEach(s => collection.Add(s));
        //        return collection;
        //    }
        //    set => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StationTypes)));
        //}

        //public ObservableCollection<Community> Communities {
        //    get {
        //        var collection = new ObservableCollection<Community>();
        //        _allCommunities.ForEach(s => collection.Add(s));
        //        return collection;
        //    }
        //    set => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Communities)));
        //}

        //public ObservableCollection<Province> Provinces {
        //    get {
        //        var collection = new ObservableCollection<Province>();
        //        _allProvinces.ForEach(s => collection.Add(s));
        //        return collection;
        //    }
        //    set => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Provinces)));
        //}

        //public ObservableCollection<District> Districts {
        //    get {
        //        var collection = new ObservableCollection<District>();
        //        _allDistricts.ForEach(s => collection.Add(s));
        //        return collection;
        //    }
        //    set => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Districts)));
        //}

        #endregion

        #region Visibilitys

        public bool StationSettingsVisibility {
            get => _stationSettingsVisibility;
            set {
                _stationSettingsVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StationSettingsVisibility)));
            }
        }

        public bool AddStationsVisibility {
            get => _addStationsVisibility;
            set {
                _addStationsVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddStationsVisibility)));
            }
        }

        public bool LoginVisibility {
            get => _loginButtonVisibility;
            set {
                _loginButtonVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoginVisibility)));
            }
        }

        public bool LogoutVisibility {
            get => _logoutButtonVisibility;
            set {
                _logoutButtonVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogoutVisibility)));
            }
        }

        public bool SettingsButtonVisibility {
            get => _stationSettingsButtonVisibility;
            set {
                _stationSettingsButtonVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SettingsButtonVisibility)));
            }
        }

        public bool AnalysisVisibility {
            get => _analysisViewVisibility;
            set {
                _analysisViewVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnalysisVisibility)));
            }
        }

        public bool StationManagerVisibilty {
            get => _stationManagerVisibilty;
            set {
                _stationManagerVisibilty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StationManagerVisibilty)));
            }
        }

        public bool RadiusVisibility {
            get => _raduisVisibility;
            set {
                _raduisVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RadiusVisibility)));
            }
        }

        public bool CommunitiesVisibility {
            get => _selectedCommunityVisibilty;
            set {
                _selectedCommunityVisibilty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CommunitiesVisibility)));
            }
        }

        public bool DistrictVisibility {
            get => _selectedDistrictVisibility;
            set {
                _selectedDistrictVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DistrictVisibility)));
            }
        }

        public bool ProvinceVisibility {
            get => _selectedProvinceVisibility;
            set {
                _selectedProvinceVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProvinceVisibility)));
            }
        }

        public bool SearchBoxVisibility {
            get => _searchBoxVisibility;
            set {
                _searchBoxVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchBoxVisibility)));
            }
        }

        #endregion

        #region Selected Vars

        public StationType SelectedStationType {
            get => _allStationTypes.Where(s => s.Id == CurrentStation.TypeId).FirstOrDefault();
            set {
                CurrentStation.TypeId = value.Id;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedStationType)));
            }
        }

        public Province SelectedProvince {
            get => _selectedProvince;
            set {
                _selectedProvince = value;
                HandleSearchWordStationChanged();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedStationType)));
            }
        }

        public District SelectedDistrict {
            get => _selectedDistrict;
            set {
                _selectedDistrict = value;
                HandleSearchWordStationChanged();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedStationType)));
            }
        }

        public Community SelectedCommunityManager {
            get => _selectedCommunity;
            set {
                _selectedCommunity = value;
                HandleSearchWordStationChanged();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedStationType)));
            }
        }

        public Community SelectedCommunity {
            get => _allCommunities.Where(s => s.Id == CurrentStation.CommunityId).FirstOrDefault();
            set {
                CurrentStation.CommunityId = value.Id;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCommunity)));
            }
        }

        public SearchType SelectedSearchType {
            get => _selectedSearchType;
            set {
                _selectedSearchType = value;
                HandleSearchTypeChanged();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSearchType)));
            }
        }

        public ModeType SelectedGroupBy {
            get => GroupByModes.Where(s => s.Mode == _groupByMode).FirstOrDefault();
            set {
                _groupByMode = value.Mode;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedGroupBy)));
            }
        }

        public ModeType SelectedCalcMode {
            get => CalcModes.Where(s => s.Mode == _calcMode).FirstOrDefault();
            set {
                _calcMode = value.Mode;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCalcMode)));
            }
        }

        public ModeType SelectedCommulationMode {
            get => CommulationModes.Where(s => s.Mode == _cummulationMode).FirstOrDefault();
            set {
                _cummulationMode = value.Mode;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCommulationMode)));
            }
        }


        #endregion

        #region Input Vars

        public string SearchWord {
            get => _searchWord;
            set {
                _searchWord = value;
                HandleSearchWordChanged();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchWord)));
            }
        }

        public string SearchWordStation {
            get => _searchWordStation;
            set {
                _searchWordStation = value;
                HandleSearchWordStationChanged();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchWordStation)));
            }
        }

        public string Radius {
            get => _radius.ToString();
            set {
                double doubleValue;
                Double.TryParse(value, out doubleValue);
                _radius = doubleValue;
                HandleRadiusChanged();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Radius)));
            }
        }

        public string SearchLat {
            get => _searchLat.ToString();
            set {
                double doubleValue;
                Double.TryParse(value, out doubleValue);
                _searchLat = doubleValue;
                HandleRadiusChanged();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchLat)));
            }
        }

        public string SearchLong {
            get => _serachLong.ToString();
            set {
                double doubleValue;
                Double.TryParse(value, out doubleValue);
                _serachLong = doubleValue;
                HandleRadiusChanged();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchLong)));
            }
        }



        public Station CurrentStation {
            get => _currentStation;
            set {
                _currentStation = value;
                if (_currentStation != null) {
                    HandleSelectedStation();
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentStation)));
            }
        }

        public string Name {
            get => _currentStation.Name;
            set {
                _currentStation.Name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public string Latitude {
            get => _currentStation.Latitude.ToString();
            set {
                double doubleValue;
                Double.TryParse(value, out doubleValue);
                _currentStation.Latitude = doubleValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Latitude)));
            }
        }

        public string Longitude {
            get => _currentStation.Longitude.ToString();
            set {
                double doubleValue;
                Double.TryParse(value, out doubleValue);
                _currentStation.Longitude = doubleValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Longitude)));
            }
        }

        public string Altitude {
            get => _currentStation.Altitude.ToString();
            set {
                double doubleValue;
                Double.TryParse(value, out doubleValue);
                _currentStation.Altitude = doubleValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Altitude)));
            }
        }




        public DateTime StartDate {
            get => _startDate;
            set {
                _startDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartDate)));
            }
        }

        public DateTime EndDate {
            get => _endDate;
            set {
                _endDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndDate)));
            }
        }

        #endregion

        #region Message Texts

        public string StationSettingsErrorText {
            get => _stationDeleteErrorText;
            set {
                _stationDeleteErrorText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StationSettingsErrorText)));
            }
        }

        public string StationSettingsSuccessText {
            get => _stationSettingsSuccessText;
            set {
                _stationSettingsSuccessText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StationSettingsSuccessText)));
            }

        }

        public string LoginText {
            get => _loginText;
            set {
                _loginText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoginText)));
            }
        }

        public string PublicErrorText {
            get => _publicUpperErrorText;
            set {
                _publicUpperErrorText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PublicErrorText)));
            }
        }

        #endregion

        #region Commands
        /*------------------------------------------------------------- C O M M A N D S -------------------------------------------------*/

        private ICommand _saveCommand;

        public ICommand SaveCommand {
            get {
                return _saveCommand ??
                       (_saveCommand = new RelayCommand(param => _stationManager.UpdateStation(_currentStation)));
            }
        }

        private ICommand _deleteCommand;

        public ICommand DeleteCommand {
            get {
                if (_deleteCommand == null) {
                    _deleteCommand = new RelayCommand(param => HandleDeleteStation());
                    _currentStation = null;
                    HandleSelectedStation();
                }

                return _deleteCommand;
            }
        }

        private ICommand _openAddNewStation;

        public ICommand OpenAddNewStation {
            get {
                return _openAddNewStation ?? (_openAddNewStation = new RelayCommand(param => HandleOpenAddNewStation()));
            }
        }

        private ICommand _addStation;

        public ICommand AddStation {
            get { return _addStation ?? (_addStation = new RelayCommand(param => HandleAddStation())); }
        }

        private ICommand _cancelAddStation;

        public ICommand CancelAddStation {
            get {
                return _cancelAddStation ?? (_cancelAddStation = new RelayCommand(param => HandleCancelAddNewStation()));
            }
        }

        private ICommand _openLogin;

        public ICommand OpenLogin {
            get { return _openLogin ?? (_openLogin = new RelayCommand(param => HandleOpenLogin())); }
        }

        private ICommand _logout;

        public ICommand Logout {
            get { return _logout ?? (_logout = new RelayCommand(param => HandleLogout())); }
        }

        private ICommand _analyticButton;

        public ICommand AnalyticButton {
            get { return _analyticButton ?? (_analyticButton = new RelayCommand(param => HandleOpenAnalytic())); }
        }

        private ICommand _stationSettingsButton;

        public ICommand StatoinSettingsButton {
            get {
                return _stationSettingsButton ??
                       (_stationSettingsButton = new RelayCommand(param => HandleOpenStationSettings()));
            }
        }

        private ICommand _openStationManager;

        public ICommand OpenStationManager {
            get {
                return _openStationManager ??
                       (_openStationManager =
                           new RelayCommand(param => HandleOpenStationManager()));
            }
        }

        private ICommand _addAll;

        public ICommand AddAllButton {
            get { return _addAll ?? (_addAll = new RelayCommand(param => AddAllButtonHandler())); }
        }

        private ICommand _clearAll;

        public ICommand ClearAllButton {
            get { return _clearAll ?? (_clearAll = new RelayCommand(param => ClearAllButtonHandler())); }
        }

        private ICommand _calculateGraphCommand;

        public ICommand CalculateGraphCommand {
            get { return _calculateGraphCommand ?? (_calculateGraphCommand = new RelayCommand(param => Task.Run(() => CalculateGraph()))); }
        }
        #endregion

        #endregion

        #region Chart

        #region Binding Props Graph

        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public Func<double, string> XFormatter { get; set; }

        #endregion

        public void GraphInit() {
            var mapper = Mappers.Xy<ValuesListEntry>()
                //.X(m => m.Date.ToFileTimeUtc())
                .X(m => m.Date.Ticks)
                .Y(m => m.Value);

            Charting.For<ValuesListEntry>(mapper);

            //XFormatter = value => DateTime.FromFileTimeUtc((long)value).ToString("MM/dd/yyyy H:mm");
            XFormatter = value => new DateTime((long)value).ToString("dd/MM/yyyy H:mm");
            _currentWindow.DataContext = _currentWindow;

        }

        private void CalculateGraph() {
            if (_startDate > _endDate) {
                PublicErrorText = "Check Input, Dates not working";
            } else {
                PublicErrorText = "Calculating graph ....";
                Thread t1 = new Thread(async () => {
                    //SeriesCollection.Clear();
                    IEnumerable<MeasurementAnalytic> measurementAnalytics = new List<MeasurementAnalytic>();
                    if (SelectedStations.Count == 0) {
                        if (SelectedCommulationMode.Mode == 0) {
                            measurementAnalytics =
                                await _measurementManager.Sum(_startDate, _endDate, _calcMode, _groupByMode);
                        } else if (SelectedCommulationMode.Mode == 1) {
                            measurementAnalytics =
                                await _measurementManager.Min(_startDate, _endDate, _calcMode, _groupByMode);
                        } else if (SelectedCommulationMode.Mode == 2) {
                            measurementAnalytics =
                                await _measurementManager.Max(_startDate, _endDate, _calcMode, _groupByMode);
                        } else if (SelectedCommulationMode.Mode == 3) {
                            measurementAnalytics =
                                await _measurementManager.Avg(_startDate, _endDate, _calcMode, _groupByMode);
                        }

                    } else {
                        if (SelectedCommulationMode.Mode == 0) {
                            measurementAnalytics = await _measurementManager.Sum(_startDate, _endDate, _calcMode,
                                _groupByMode,
                                SelectedStations);
                        } else if (SelectedCommulationMode.Mode == 1) {
                            measurementAnalytics = await _measurementManager.Min(_startDate, _endDate, _calcMode,
                                _groupByMode,
                                SelectedStations);
                        } else if (SelectedCommulationMode.Mode == 2) {
                            measurementAnalytics = await _measurementManager.Max(_startDate, _endDate, _calcMode,
                                _groupByMode,
                                SelectedStations);
                        } else if (SelectedCommulationMode.Mode == 3) {
                            measurementAnalytics = await _measurementManager.Avg(_startDate, _endDate, _calcMode,
                                _groupByMode,
                                SelectedStations);
                        }
                    }



                    _context.Send(x => {
                        ChartValues<ValuesListEntry> chartValues = new ChartValues<ValuesListEntry>();


                        int count = 0;
                        List<ValuesListEntry> valuesListEntries = new List<ValuesListEntry>();
                        if (measurementAnalytics != null) {
                            foreach (MeasurementAnalytic measurementAnalytic in measurementAnalytics) {
                                // Debug.WriteLine($"{measurementAnalytic.Day} {measurementAnalytic.Month} {measurementAnalytic.Year} {measurementAnalytic.Hour} Value {measurementAnalytic.Value} {dateTime.ToString()}");
                                if (count < 1001) {
                                    valuesListEntries.Add(new ValuesListEntry(measurementAnalytic.Value,
                                        measurementAnalytic.GetDateTime()));
                                    //SeriesCollection[0].Values.Add(new ValuesListEntry(measurementAnalytic.Value,
                                    //    measurementAnalytic.GetDateTime()));
                                } else {
                                    break;
                                }

                                count++;
                            }

                            PublicErrorText = "";


                        } else {
                            PublicErrorText = "Nothing found";
                        }

                        if (count < 1) {
                            PublicErrorText = "Nothing found";
                        } else {
                            chartValues.AddRange(valuesListEntries);
                        }


                        SeriesCollection.Clear();
                        SeriesCollection.Add(new LineSeries {
                            Title = "result",
                            Values = chartValues
                        });

                        if (count >= 1000) {
                            PublicErrorText = "Please choose smaller date range!";
                        }

                    }, null);

                });

                t1.Start();
            }
        }




        #endregion

        #region Handlers

        #region Operners

        private void HandleOpenAnalytic() {
            AnalysisVisibility = true;
            StationSettingsVisibility = false;
            AddStationsVisibility = false;
            StationManagerVisibilty = false;
            PublicErrorText = "";
        }

        private void HandleOpenStationSettings() {
            AnalysisVisibility = false;
            StationSettingsVisibility = true;
            AddStationsVisibility = false;
            StationManagerVisibilty = false;
            PublicErrorText = "";
        }

        private void HandleOpenLogin() {
            LoginWindow login = new LoginWindow();
            login.Show();
            _currentWindow.Close();
        }

        private void HandleLogout() {
            CockpitWindow window = new CockpitWindow();
            window.Show();
            _currentWindow.Close();
        }

        private async void HandleCancelAddNewStation() {
            AddStationsVisibility = false;
            StationSettingsVisibility = true;
            _currentStation = null;
            HandleSelectedStation();
            PublicErrorText = "";
        }

        #endregion

        #region Reactors

        private void HandleOpenAddNewStation() {
            AddStationsVisibility = true;
            StationSettingsVisibility = false;
            AnalysisVisibility = false;
            _currentStation = null;
            HandleSelectedStation();
            _currentStation = new Station();
        }

        private async void HandleAddStation() {
            _currentStation.Creator = _currentUser.Id;
            if (await _stationManager.AddStation(_currentStation)) {
                _currentStation = null;
                HandleSelectedStation();
                _currentStation = new Station();
                LoadStations();
                StationSettingsSuccessText = "New station added successful";
            } else {
                StationSettingsErrorText = "Check input, error while adding new station";
            }
        }

        private async void HandleDeleteStation() {
            if (await _stationManager.DeleteStation(_currentStation)) {
                StationSettingsErrorText = "";
                _currentStation = null;
                LoadStations();
                HandleSelectedStation();
            } else {
                StationSettingsErrorText = "Delete not possible, measurements for station existing";
            }
        }

        private void HandleSearchWordChanged() {
            _context.Send((x) => {
                Stations.Clear();
                string searchWordLower = SearchWord?.ToLower();
                _allStations.Where(p => p.Name.ToLower().Contains(searchWordLower)).ToList()
                    .ForEach(p => Stations.Add(p));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Stations)));
            }, null);
        }

        private void HandleSelectedStation() {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Latitude)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Longitude)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Altitude)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedStationType)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCommunity)));
            StationSettingsErrorText = "";
            StationSettingsSuccessText = "";
        }

        #endregion

        #region Loaders

        public async void LoadStations() {
            _context.Send(x => Stations.Clear(), null);
            //_allStations.Clear();
            SearchWord = "";
            foreach (Station station in await _stationManager.GetStationByCreatorId(_currentUser.Id)) {
                _context.Send((x) => {
                    Stations.Add(station);
                    //_allStations.Add(station);
                }, null);
            }
        }

        public async void LoadAllStations() {
            _context.Send(x => _allStations.Clear(), null);

            foreach (Station station in await _stationManager.GetAllStations()) {
                _context.Send(x => _allStations.Add(station), null);
            }
        }

        public async void LoadTypes() {
            _context.Send(x => _allStationTypes.Clear(), null);
            foreach (StationType stationType in await _stationManager.GetAllStationTypes()) {
                _context.Send(
                    (x) => {
                        _allStationTypes.Add(stationType);
                        StationTypes.Add(stationType);
                    }, null);

            }
        }

        public async void LoadCommunities() {
            _context.Send(async x => {
                _allCommunities.Clear();
                _allDistricts.Clear();
                _allProvinces.Clear();

                foreach (Community community in await _stationManager.GetAllCommunities()) {
                    _allCommunities.Add(community);
                    Communities.Add(community);
                }
                _selectedCommunity = _allCommunities.First();

                foreach (District district in await _stationManager.GetAllDistricts()) {
                    _allDistricts.Add(district);
                    Districts.Add(district);
                }
                _selectedDistrict = _allDistricts.First();

                foreach (Province province in await _stationManager.GetAllProvinces()) {
                    _allProvinces.Add(province);
                    Provinces.Add(province);
                }
                _selectedProvince = _allProvinces.First();
            }, null);

            //_context.Send(x => , null);
            //InvokeLoadedReady();
        }

        private void InvokeLoadedReady() {
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Communities)));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Provinces)));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Districts)));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StationTypes)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCommunityManager)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDistrict)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedProvince)));

        }

        private void LoadModes() {
            CalcModes.Add(new ModeType("Temperature", QueryMode.temperature));
            CalcModes.Add(new ModeType("Air Pressure", QueryMode.air_pressure));
            CalcModes.Add(new ModeType("Rainfall", QueryMode.rainfall));
            CalcModes.Add(new ModeType("Humidity", QueryMode.humidity));
            CalcModes.Add(new ModeType("Wind Speed", QueryMode.wind_speed));

            GroupByModes.Add(new ModeType("Day", GroupByMode.day));
            GroupByModes.Add(new ModeType("None", GroupByMode.none));
            GroupByModes.Add(new ModeType("Hour", GroupByMode.hour));
            GroupByModes.Add(new ModeType("Week", GroupByMode.week));
            GroupByModes.Add(new ModeType("Month", GroupByMode.month));
            GroupByModes.Add(new ModeType("Year", GroupByMode.year));

            CommulationModes.Add(new ModeType("Sum", 0));
            CommulationModes.Add(new ModeType("Min", 1));
            CommulationModes.Add(new ModeType("Max", 2));
            CommulationModes.Add(new ModeType("Average", 3));

            SelectedCalcMode = CalcModes.First();
            SelectedGroupBy = GroupByModes.First();
            SelectedCommulationMode = CommulationModes.First();


        }

        private void LoadSearchTypes() {
            SearchTypes.Add(new SearchType("By Name", NameSearchTypeHandler));
            SearchTypes.Add(new SearchType("By Region", RegionSearchTypeHandler));
            SearchTypes.Add(new SearchType("By Community", CommunitySearchTypeHandler));
            SearchTypes.Add(new SearchType("By District", DistrictSearchTypeHandler));
            SearchTypes.Add(new SearchType("By Province", ProvinceSearchTypeHandler));
            SelectedSearchType = SearchTypes.First();
        }

        #endregion

        #region StationManager

        #region Reactors

        private void AddAllButtonHandler() {
            foreach (Station searchListStation in SearchListStations) {
                if (!SelectedStations.Contains(searchListStation)) {
                    SelectedStations.Add(searchListStation);
                }
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedStations)));
        }

        private void ClearAllButtonHandler() {
            SelectedStations.Clear();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedStations)));
        }

        private void HandleSearchTypeChanged() {
            SearchWordStation = "";
            SelectedSearchType.Func();
        }

        private void HandleSearchWordStationChanged() {
            SelectedSearchType.Func();
        }

        private void HandleRadiusChanged() {
            SelectedSearchType.Func();
        }


        #endregion

        #region ModeHandlers

        private void NameSearchTypeHandler() {
            if (_allStations != null) {
                SearchBoxVisibility = true;
                RadiusVisibility = CommunitiesVisibility = ProvinceVisibility = DistrictVisibility = false;
                SearchListStations.Clear();
                string searchWordLower = SearchWordStation?.ToLower();
                _allStations.Where(p => p.Name.ToLower().Contains(searchWordLower)).ToList()
                    .ForEach(p => SearchListStations.Add(p));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchListStations)));
            }

        }

        private async void RegionSearchTypeHandler() {
            SearchListStations.Clear();
            RadiusVisibility = true;
            SearchBoxVisibility = CommunitiesVisibility = ProvinceVisibility = DistrictVisibility = false;

            IEnumerable<Station> stations =
                await _stationManager.FilterByRegion(new GeoCoordinate(_searchLat, _serachLong), _radius);
            if (stations != null) {
                foreach (Station station in stations) {
                    SearchListStations.Add(station);
                }
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchListStations)));
        }

        private async void CommunitySearchTypeHandler() {
            if (_allCommunities != null) {

                SearchListStations.Clear();
                CommunitiesVisibility = true;
                SearchBoxVisibility = RadiusVisibility = ProvinceVisibility = DistrictVisibility = false;
                IEnumerable<Station> stations = await _stationManager.FilterByRegion(SelectedCommunityManager);
                if (stations != null) {
                    foreach (Station station in stations) {
                        SearchListStations.Add(station);
                    }
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchListStations)));
            }
        }

        private async void DistrictSearchTypeHandler() {
            if (_allDistricts != null) {
                SearchListStations.Clear();
                DistrictVisibility = true;
                SearchBoxVisibility = RadiusVisibility = ProvinceVisibility = CommunitiesVisibility = false;
                IEnumerable<Station> stations = await _stationManager.FilterByRegion(SelectedDistrict);
                if (stations != null) {
                    foreach (Station station in stations) {
                        SearchListStations.Add(station);
                    }
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchListStations)));
            }
        }

        private async void ProvinceSearchTypeHandler() {
            if (_allProvinces != null) {
                SearchListStations.Clear();
                ProvinceVisibility = true;
                SearchBoxVisibility = RadiusVisibility = DistrictVisibility = CommunitiesVisibility = false;
                IEnumerable<Station> stations = await _stationManager.FilterByRegion(SelectedProvince);
                if (stations != null) {
                    foreach (Station station in stations) {
                        SearchListStations.Add(station);
                    }
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchListStations)));
            }
        }

        #endregion

        #region Openers

        private void HandleOpenStationManager() {
            StationManagerVisibilty = true;
            AnalysisVisibility = false;
            PublicErrorText = "";
            InvokeLoadedReady();
        }

        #endregion

        #endregion

        //private void HandlePropChanged(object sender, PropertyChangedEventArgs e) {

        //    if(sender.Equals(this) && e.PropertyName.Equals(nameof(SearchWord))) {
        //        Stations.Clear();
        //        string searchWordLower = SearchWord?.ToLower();
        //        AllStations.Where(p => p.Name.ToLower().Contains(searchWordLower)).ToList()
        //            .ForEach(p => Stations.Add(p));
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Stations)));
        //    }
        //}

        #endregion

        //https://lvcharts.net/App/examples/wpf/start

    }

    public class ValuesListEntry : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private double _value;
        private DateTime _date;

        public ValuesListEntry(double value, DateTime date) {
            _value = value;
            _date = date;
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
}
