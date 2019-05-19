using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Render;
using RadarView.Model.Service.AircraftDataSourceService;
using RadarView.Model.Service.SessionContext;
using RadarView.ViewModel.MainWindow;
using Unity;
using Unity.Resolution;

namespace RadarView.View
{
    /// <summary>
    /// Hlavni okno aplikace.
    /// </summary>
    public partial class MainWindow : Window, IMainWindowViewModel
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        private readonly Renderer _renderer;

        public MainWindow(MainWindowViewModel mainWindowViewModel, Renderer renderer)
        {
	        this.InitializeComponent();

	        this.DataContext = mainWindowViewModel;
	        this._mainWindowViewModel = mainWindowViewModel;
	        this._renderer = renderer;
			this._renderer.Loaded += this._renderer_Loaded;
		}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
	        this.gridRadarView.Children.Add(this._renderer);
	        //EvaluateAppArgs(dataSourceService, boundingBox);
        }

        private void _renderer_Loaded(object sender, RoutedEventArgs e)
        {
	        this._mainWindowViewModel.Initialize();
        }
	}
}
