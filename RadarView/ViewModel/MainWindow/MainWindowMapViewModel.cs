using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json;
using RadarView.Model.Entities.MapLayer;
using RadarView.Properties;

namespace RadarView.ViewModel.MainWindow
{
	/// <summary>
	/// ViewModel pro mapy.
	/// </summary>
	partial class MainWindowViewModel
	{
		/// <summary>
		/// Mapové podklady.
		/// </summary>
		private MapLayerCollection mapLayers;

		/// <summary>
		/// Kolekce tlačítek horního meny, ktere se starají o ovládání mapových podkladu.
		/// </summary>
		private ObservableCollection<MenuItem> _mapMenuItems = new ObservableCollection<MenuItem>();
		public ObservableCollection<MenuItem> MapMenuItems
		{
			get { return this._mapMenuItems; }
			set
			{
				this._mapMenuItems = value; 
				this.OnPropertyChanged(nameof(this.MapMenuItems));
			}
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		partial void MapViewModelInit()
		{
			this.mapLayers = this._sessionContext.MapLayerCollection;
			this.CreateItemMenuForMaps();
		}

		/// <summary>
		/// Vytvoří tlačítka horního meny, pro každou načtenou mapu.
		/// </summary>
		private void CreateItemMenuForMaps()
		{
			var mapLayers = this.mapLayers.Layers;
			for (var i = 0; i < mapLayers.Count; i++) {
				var mapLayer = mapLayers[i];
				var menuItem = new MenuItem {Header = mapLayer.Name};
				menuItem.IsCheckable = true;
				menuItem.Click += this.MenuItem_Click;
				menuItem.Tag = i;

				this._mapManager.ChangeVisibility(mapLayer.Name, mapLayer.IsVisible);
				menuItem.IsChecked = mapLayer.IsVisible;

				this.MapMenuItems.Add(menuItem);;
			}
		}

		private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var menuItem = (MenuItem)sender;
			var index = (int)menuItem.Tag;

			var clickedLayer = this.mapLayers.Layers[index];
			clickedLayer.IsVisible = menuItem.IsChecked;
			this._mapManager.ChangeVisibility(clickedLayer.Name, menuItem.IsChecked);
		}


		/// <summary>
		/// Uloží informace o mapách do konfigu.
		/// </summary>
		partial void StoreMapInfoToConfig()
		{
			var json = JsonConvert.SerializeObject(this.mapLayers);
			//Json konverter trošku zlobí, neumí pracovat s dictionary, takže tam přidává znaky navíc.
			json = json.Replace("\\", string.Empty);
			json = json.Replace("\"{", "{");
			json = json.Replace("}\"", "}");
			Settings.Default.MapConfig = json;
		}
	}
}
