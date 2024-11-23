/*
  Application Name:     Unitgate  
  Created by:           Allan Wamming Lie
  Created:              2014-2025
  Company:              Lie Consulting
  E-mail:               unitgate@allanlie.dk 
  Web:                  unitgate.eu
 */

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using UnitGate.DemoData;
using UnitGate.Helpers;
using UnitGate.Models;
using UnitGate.Enums;
using UnitGate.Service;
using UnitGate.View;

namespace UnitGate.ViewModel
{
  internal class MainViewModel : INotifyPropertyChanged
  {
    private static readonly object _itemsLock = new object();
    private static readonly object _thisLock = new object();
    private readonly DelegateCommand<string> _aboutCommand;
    private readonly DelegateCommand<string> _staticIPCommand;
    private readonly DelegateCommand<string> _exitCommand;
    private readonly DelegateCommand<string> _dataConfigCommand;
    private readonly DelegateCommand<string> _exportToExcelCommand;
    private readonly DelegateCommand<string> _exportRawDataCommand;
    private ConfigService _configService;
    private Brush _connectedColor;
    private string _connectionStatus;
    private string _title;
    private string _gatewayIp;
    private int _inverterCount;
    private IpService _ipService;
    private Visibility _searchingForIp;
    private double _totalACWatt;
    private double _totalDCWatt;
    private double _totalLifeKwh;
    private GatewayService _gatewayService;
    private int _appTry;

    public MainViewModel()
    {
      var obj = Assembly.GetExecutingAssembly().GetName().Version;
      Title = $"UnitGate - Version {obj.Major}.{obj.Minor}.{obj.Build}";

      Task.Run(() => InitiliseServices());
      ConnectionStatus = "Disconnected";
      ConnectedColor = Brushes.Red;

      Zigbits = new ObservableCollection<Zigbit>();
      BindingOperations.EnableCollectionSynchronization(Zigbits, _itemsLock);
      //LoadDemoData();

      _dataConfigCommand = new DelegateCommand<string>(DataConfigCommandExecute);
      _exitCommand = new DelegateCommand<string>(ExitCommandExecute);
      _aboutCommand = new DelegateCommand<string>(AboutCommandExecute);
      _staticIPCommand = new DelegateCommand<string>(StaticIPCommandExecute);
      _exportToExcelCommand = new DelegateCommand<string>(ExportToExcelCommandExecute);
      _exportRawDataCommand = new DelegateCommand<string>(ExportRawDataCommandExecute);

    }


    public ObservableCollection<Zigbit> Zigbits { get; private set; }

    public DelegateCommand<string> DataConfigCommand
    {
      get { return _dataConfigCommand; }
    }

    public DelegateCommand<string> ExitCommand
    {
      get { return _exitCommand; }
    }

    public DelegateCommand<string> AboutCommand
    {
      get { return _aboutCommand; }
    }
    public DelegateCommand<string> StaticIPCommand
    {
      get { return _staticIPCommand; }
    }
    public DelegateCommand<string> ExportToExcelCommand
    {
      get { return _exportToExcelCommand; }
    }
    public DelegateCommand<string> ExportRawDataCommand
    {
      get { return _exportRawDataCommand; }
    }


    public string Title
    {
      get { return _title; }
      set
      {
        _title = value;
        RaisePropertyChanged("Title");
      }
    }


    public string GatewayIp
    {
      get { return _gatewayIp; }
      set
      {
        _gatewayIp = value;
        RaisePropertyChanged("GatewayIp");
      }
    }

    public Visibility SearchingForIp
    {
      get { return _searchingForIp; }
      set
      {
        _searchingForIp = value;
        RaisePropertyChanged("SearchingForIp");
      }
    }

    public string ConnectionStatus
    {
      get { return _connectionStatus; }
      set
      {
        _connectionStatus = value;
        RaisePropertyChanged("ConnectionStatus");
      }
    }

    public Brush ConnectedColor
    {
      get { return _connectedColor; }
      set
      {
        _connectedColor = value;
        RaisePropertyChanged("ConnectedColor");
      }
    }

    public double TotalDCWatt
    {
      get { return _totalDCWatt; }
      set
      {
        _totalDCWatt = value;
        RaisePropertyChanged("TotalDCWatt");
      }
    }

    public double TotalACWatt
    {
      get { return _totalACWatt; }
      set
      {
        _totalACWatt = value;
        RaisePropertyChanged("TotalACWatt");
      }
    }

    public double TotalLifeKwh
    {
      get { return _totalLifeKwh; }
      set
      {
        _totalLifeKwh = value;
        RaisePropertyChanged("TotalLifeKwh");
      }
    }

    public int InverterCount
    {
      get { return _inverterCount; }
      set
      {
        _inverterCount = value;
        RaisePropertyChanged("InverterCount");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    private void DataConfigCommandExecute(string args)
    {
      var dataConfigView = new DataConfigView();
      dataConfigView.Show();
    }

    private void ExitCommandExecute(string args)
    {
      Application.Current.Windows[0].Close();
    }

    private void AboutCommandExecute(string args)
    {
      var aboutView = new AboutView();
      aboutView.Show();
    }
    private void StaticIPCommandExecute(string args)
    {
      var staticIPView = new StaticIPView();
      staticIPView.Show();
    }
    private void ExportToExcelCommandExecute(string obj)
    {
      ExportService.ExportToExcel(Zigbits.ToList());

    }

    private void ExportRawDataCommandExecute(string obj)
    {
      ExportService.ExportRawData(Zigbits.ToList());
    }


    private async void InitiliseServices()
    {
      try
      {
        _appTry++;
        _configService = ConfigService.Instance;
        _ipService = new IpService();
        var storedIp = _configService.GetConfig("IpAddress", ConfigTypes.General);
        var gatewayIp = "";
        if (string.IsNullOrEmpty(storedIp))
        {
          gatewayIp = await _ipService.GetGatewayIp();
          _configService.AppendConfig("IpAddress", gatewayIp, ConfigTypes.General);
          _configService.CommitConfig(ConfigTypes.General);
        }
        else
        {
          bool isStoredIpValid = await _ipService.ValidateIpAsync(storedIp);
          if (!isStoredIpValid)
          {
            gatewayIp = await _ipService.GetGatewayIp();
            _configService.AppendConfig("IpAddress", gatewayIp, ConfigTypes.General);
            _configService.CommitConfig(ConfigTypes.General);
          }
        }

        if(!string.IsNullOrEmpty(gatewayIp))
        {
          GatewayIp = gatewayIp;
          SearchingForIp = Visibility.Collapsed;
          ConnectionStatus = "Connected";
          ConnectedColor = Brushes.Green;
          _gatewayService = new GatewayService(gatewayIp);

          _gatewayService.OnZigbitUpdated += OnZigbitUpdated;

          await Task.Run(() => _gatewayService.StartDataCollection());
        } else
        {
          RetryErrorFlow();
        }
      }
      catch (Exception)
      {
        RetryErrorFlow();
      }
    }

    private async void RetryErrorFlow()
    {
      if (_appTry < 4)
      {
        await Application.Current.Dispatcher.BeginInvoke(
           DispatcherPriority.Background,
           new Action(InitiliseServices));
      }
      else
      {
        await Application.Current.Dispatcher.BeginInvoke(
           DispatcherPriority.Background,
           new Action(InitError));

      }
    }

    private void InitError()
    {
      MessageBox.Show("An error has occoured. Please try to run the application again \nIf the error keeps occuring please remove and reinstall the application");

      var window = Application.Current.Windows[0];
      if (window != null) window.Close();
    }

    private async void OnZigbitUpdated(Zigbit newItem)
    {
      await Task.Run(() =>
      {
        try
        {
          if (newItem != null)
          {
            lock (_thisLock)
            {
              string[] filter = _configService.GetConfig(newItem.Serial, ConfigTypes.Data).Split(';');
              string alias = filter[0];
              bool exclude = false;
              if (filter.Length > 1)
                exclude = bool.Parse(filter[1] ?? "false");


              if (_configService.GetConfigDictionary(ConfigTypes.Data).ContainsKey(newItem.Serial))
              {
                if (!string.IsNullOrEmpty(alias))
                {
                  newItem.Alias = alias;
                }
              }
              else
              {
                _configService.AppendConfig(newItem.Serial, string.Empty, ConfigTypes.Data);
                _configService.CommitConfig(ConfigTypes.Data);
              }

              var oldItem = Zigbits.FirstOrDefault(x => x.Serial == newItem.Serial);
              if (oldItem != null)
              {
                var oldIndex = Zigbits.IndexOf(oldItem);

                if (!exclude)
                {

                  Zigbits[oldIndex] = newItem;
                }
                else
                {
                  Zigbits.RemoveAt(oldIndex);
                }
              }
              else
              {
                if (!exclude)
                  Zigbits.Add(newItem);
              }


              CalculateTotaProduction();
            }
          }

        }
        catch (Exception ex)
        {
          ErrorHandlingService.PersistError("MainViewModel - OnZigbitUpdated", ex);


        }
      });
    }

    private void LoadDemoData(int times = 1)
    {
      for (int i = 0; i < times; i++)
      {
        foreach (Zigbit zigbit in ZigbitDemoData.LoadDemoData())
        {
          Zigbits.Add(zigbit);
        }
      }
      CalculateTotaProduction();
    }

    private void CalculateTotaProduction()
    {
      double totalDC = 0, totalAc = 0, totalLife = 0;

      var tempZig = Zigbits;

      foreach (var zigbit in tempZig)
      {
        totalDC += zigbit.DCWatt;
        totalAc += zigbit.ACWatt;
        totalLife += zigbit.LifeProduction;
      }

      TotalDCWatt = totalDC;
      TotalACWatt = totalAc;
      TotalLifeKwh = totalLife;
      InverterCount = Zigbits.Count;
    }

    private void RaisePropertyChanged(string propertyName)
    {
      var handlers = PropertyChanged;

      handlers(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}