using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

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
    }

    private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
    {
      var url = "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=5USY5P3QBV2X2";
      try
      {
        Process.Start(new ProcessStartInfo
        {
          FileName = url,
          UseShellExecute = true
        });
      }
      catch (Exception)
      {
        // Do Nothing
      }
    }

  }
}