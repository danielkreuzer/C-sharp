using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Wetr.Domain;
using Wetr.Server.Factory;
using Wetr.Server.Interface;

namespace Wetr.Cockpit.Gui {
    /// <summary>
    /// Interaction logic for CockpitWindow.xaml
    /// </summary>
    public partial class CockpitWindow : MetroWindow {
        private IUserManager userManager = UserManagerFactory.GetUserManager();
        private IStationDataManager stationManager = StationDataManagerFactory.GetStationDataManager();
        private IMeasurementManager measurementManager = MeasurementManagerFactory.GetMeasurementManager();

         
        public CockpitWindow(User user) {
            InitializeComponent();
            this.Loaded += (s, e) => { DataContext = new CockpitVM(stationManager, userManager, measurementManager, user, this); };
        }

        public CockpitWindow() {
            InitializeComponent();
            this.Loaded += (s, e) => { DataContext = new CockpitVM(stationManager, userManager, measurementManager, this); };
        }

    }
}
