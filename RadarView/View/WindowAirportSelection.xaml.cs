using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xna.Framework;
using RadarView.Model.DataService.AviationData.AirportDataService;
using RadarView.Model.Entities.AviationData.Airports;
using RadarView.Properties;
using RadarView.ViewModel.WindowAirportSelection;
using BoundingBox = RadarView.Model.Entities.Geographic.BoundingBox;

namespace RadarView.View
{
	/// <summary>
	/// Formulář pro výběr letiště.
	/// </summary>
	public partial class WindowAirportSelection : Window, IWindowAirportSelectionViewModel
	{
		public WindowAirportSelection(IWindowAirportSelectionViewModel airportSelectionViewModel)
		{
			this.InitializeComponent();
			this.DataContext = airportSelectionViewModel;
		}
	}
}
