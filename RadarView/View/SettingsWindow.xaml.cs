using System.Windows;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Service.AircraftDataSourceService;
using RadarView.Properties;
using RadarView.ViewModel.MainWindow;
using RadarView.ViewModel.WindowSettings;

namespace RadarView.View
{
    /// <summary>
    /// Okno pro změnu konfiguračních hodnot.
    /// </summary>
    public partial class SettingsWindow : Window, IWindowsSettingsViewModel
    {
        public SettingsWindow(IWindowsSettingsViewModel windowsSettingsViewModel)
        {
            this.InitializeComponent();
            this.DataContext = windowsSettingsViewModel;
        }
    }
}
