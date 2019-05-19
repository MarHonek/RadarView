using System.ComponentModel;
using System.Runtime.CompilerServices;
using RadarView.Annotations;

namespace RadarView.ViewModel.Base
{
	/// <summary>
	/// Base ViewModel pro MVVM.
	/// </summary>
	public class BaseViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
