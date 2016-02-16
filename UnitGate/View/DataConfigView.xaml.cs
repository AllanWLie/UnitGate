using CSharpAnalytics;

namespace UnitGate.View
{
    /// <summary>
    ///     Interaction logic for ConfigIPView.xaml
    /// </summary>
    public partial class DataConfigView
    {
        public DataConfigView()
        {
            InitializeComponent();

            AutoMeasurement.Client.TrackScreenView("DataConfigView");
        }
    }
}