using CSharpAnalytics;

namespace UnitGate.View
{
    /// <summary>
    /// Interaction logic for StaticIPView.xaml
    /// </summary>
    public partial class StaticIPView
    {
        public StaticIPView()
        {
            InitializeComponent();

            AutoMeasurement.Client.TrackScreenView("StaticIPView");
        }
    }
}
