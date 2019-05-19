using System.Windows;

namespace RadarView.View
{
    /// <summary>
    /// Okno s informacemi o aplikaci.
    /// </summary>
    public partial class AboutBox : Window
    {
        public AboutBox()
        {
            this.InitializeComponent();
        }

		private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
		{
			System.Diagnostics.Process.Start(e.Uri.ToString());
		}
	}
}
