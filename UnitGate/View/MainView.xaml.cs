using System.Diagnostics;
using System.Windows.Input;
using CSharpAnalytics;

namespace UnitGate.View
{
    /// <summary>
    ///     Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView
    {
       
        public MainView()
        {
            InitializeComponent();

            AutoMeasurement.Instance = new WpfAutoMeasurement();
            AutoMeasurement.DebugWriter = d => Debug.WriteLine(d);
            AutoMeasurement.Start(new MeasurementConfiguration("UA-39367325-3"));

            AutoMeasurement.Client.TrackScreenView("MainView");
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var url = "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=5USY5P3QBV2X2";

            Process.Start(url);
        }

    }
}