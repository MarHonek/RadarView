using RadarView.Properties;
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

namespace RadarView
{
    /// <summary>
    /// Interaction logic for ReplayTimeWindow.xaml
    /// </summary>
    public partial class ReplayDateWindow : Window
    {
        public event EventHandler<ReplayEventArgs> TimeSelected;

        private DateTime defaultDateTime;

        public ReplayDateWindow()
        {
            InitializeComponent();      
        }
        
        public ReplayDateWindow(DateTime defaultDateTime) : this()
        {
            this.defaultDateTime = Settings.Default.LocalTimeZone ? defaultDateTime.ToLocalTime() : defaultDateTime;
            Init();
        }

        private void Init()
        {
            datePickerReplay.SelectedDate = defaultDateTime;
            intergerUpDownReplayHour.Value = defaultDateTime.Hour;
            intergerUpDownReplayMinute.Value = defaultDateTime.Minute;
        }

        private void buttonReplayOk_Click(object sender, RoutedEventArgs e)
        {
            if(datePickerReplay.SelectedDate.HasValue)
            {
                DateTimeOffset selectedDateTime = datePickerReplay.SelectedDate.Value.AddHours(intergerUpDownReplayHour.Value.Value).AddMinutes(intergerUpDownReplayMinute.Value.Value).ToUniversalTime();
                DateTime dateTimeNow = DateTime.UtcNow;
                if (selectedDateTime >= dateTimeNow)
                {
                    MessageBox.Show("Vybraný čas musí být menší než aktuální", "Chyba", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                else
                {
                    DateTime date = datePickerReplay.SelectedDate.Value.AddHours(intergerUpDownReplayHour.Value.Value).AddMinutes(intergerUpDownReplayMinute.Value.Value);

                    if (Settings.Default.LocalTimeZone)
                    {
                        TimeSelected?.Invoke(this, new ReplayEventArgs(date.ToUniversalTime()));
                    } else
                    {
                        TimeSelected?.Invoke(this, new ReplayEventArgs(date));
                    }
                    this.Close();
                }
            } else
            {
                MessageBox.Show("Musíte vybrat datum", "Chyba", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }
    }

    public class ReplayEventArgs : EventArgs
    {
        public DateTime Date { get; set; }

        public ReplayEventArgs(DateTime date)
        {
            this.Date = date;
        }
    }
}
