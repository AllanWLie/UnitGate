using System.Diagnostics;
using System.Windows.Input;
using CSharpAnalytics;

namespace UnitGate.View
{
    /// <summary>
    ///     Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView
    {
        public AboutView()
        {
            InitializeComponent();
            AutoMeasurement.Client.TrackScreenView("AboutView");
        }

        private void LinkedInMouseDown(object sender, MouseButtonEventArgs e)
        {
            var url = "https://dk.linkedin.com/pub/allan-lie/24/1b6/5b1";

            Process.Start(url);
        }

        private void WebMouseDown(object sender, MouseButtonEventArgs e)
        {
            var url = "http://www.allanlie.dk";

            Process.Start(url);
        }

        private void MailMouseDown(object sender, MouseButtonEventArgs e)
        {
            var url = "mailto://allan@allanlie.dk";

            Process.Start(url);
        }
    }
}