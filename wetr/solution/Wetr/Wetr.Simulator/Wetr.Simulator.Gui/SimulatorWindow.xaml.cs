using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Wetr.Simulator.Gui.ViewModel;
using Wetr.Simulator.Logic;

namespace Wetr.Simulator.Gui {
    /// <summary>
    /// Interaction logic for SimulatorWindow.xaml
    /// </summary>
    public partial class SimulatorWindow : MetroWindow {
        private readonly ISimulator _simulator = SimulatorFactory.GetSimulator();

        public SimulatorWindow() {
            InitializeComponent();

            this.Loaded += (s, e) => DataContext = new SimulatorVM(_simulator);
            this.Closed += (s, e) => _simulator.StopSimulator();
        }

        private void NumberInput_OnPreviewTextInput(object sender, TextCompositionEventArgs e) {
            // get old content plus the current addition
            string text = ((TextBox)sender).Text + e.Text;

            e.Handled = !IsTextAllowed(text);
        }

        private static bool IsTextAllowed(string text) {
            // matches valid numbers 
            Regex regex = new Regex(@"^-?(0|[1-9]\d*)?(\.?\,?(\d+)?)?$");

            return regex.IsMatch(text);
        }
    }

    public class TabDataTemplateSelector : DataTemplateSelector {
        public DataTemplate SettingsTemplate { get; set; }
        public DataTemplate StationTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            if (container is FrameworkElement && item is IViewModelBase) {
                switch (item) {
                    case SettingsVM _:
                        return SettingsTemplate;
                    case StationVM _:
                        return StationTemplate;
                }
            }

            return null;
        }
    }

    public class InverseBooleanToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null) return Visibility.Collapsed;
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }

    public class InverseBooleanConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is bool b) {
                return !b;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }

    public class BooleanAndConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            foreach (object value in values) {
                if ((value is bool b) && b == false) {
                    return false;
                }
            }
            return true;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
