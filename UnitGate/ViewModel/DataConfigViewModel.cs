/*
  Application Name:     Unitgate  
  Created by:           Allan Wamming Lie
  Created:              2014-2016
  Company:              Lie Development
  E-mail:               unitgate@allanlie.dk 
  Web:                  unitgate.eu
 */

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Shared.Helpers;
using UnitGate.Enums;
using UnitGate.Service;

namespace UnitGate.ViewModel
{
    internal class DataConfigViewModel : INotifyPropertyChanged
    {
        private static readonly object _itemsLock = new object();
        private readonly DelegateCommand<string> _closeButtonCommand;
        private readonly DelegateCommand<string> _saveButtonCommand;
        private readonly ConfigService _configService;
        private readonly char splitChar = ';';

        public ObservableCollection<DataConfigEntryViewModel> Inverters { get; set; }

        public DataConfigViewModel()
        {
            Inverters = new ObservableCollection<DataConfigEntryViewModel>();
            BindingOperations.EnableCollectionSynchronization(Inverters, _itemsLock);

            _configService = ConfigService.Instance;
            if (_configService.GetConfigDictionary(ConfigTypes.Data).Count > 0)
            {
                foreach (var pair in _configService.GetConfigDictionary(ConfigTypes.Data))
                {
                    var dataConfigViewModel = new DataConfigEntryViewModel();
                    dataConfigViewModel.SerialNumber = pair.Key;

                    string[] filter = pair.Value.Split(splitChar);
                    dataConfigViewModel.Alias = filter[0];
                    bool exclude = false;
                    if (filter.Length > 1)
                        exclude = bool.Parse(filter[1] ?? "false");

                    dataConfigViewModel.Exclude = exclude;

                    Inverters.Add(dataConfigViewModel);
                }
            }

            _saveButtonCommand = new DelegateCommand<string>(SaveButtonCommandExecute, SaveButtonCommandCanExecute);

            _closeButtonCommand = new DelegateCommand<string>(CloseButtonCommandExecute);

        }



        public DelegateCommand<string> SaveButtonCommand
        {
            get { return _saveButtonCommand; }
        }

        public DelegateCommand<string> CloseButtonCommand
        {
            get { return _closeButtonCommand; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private async void OnAliasConfigUpdated(KeyValuePair<string, string> config)
        {
            await Task.Run(() =>
            {
                var dataConfigEntryViewModel = new DataConfigEntryViewModel { SerialNumber = config.Key };

                string[] filter = config.Value.Split(splitChar);
                dataConfigEntryViewModel.Alias = filter[0];
                bool exclude = false;
                if (filter.Length > 1)
                    exclude = bool.Parse(filter[1] ?? "false");

                dataConfigEntryViewModel.Exclude = exclude;

                Inverters.Add(dataConfigEntryViewModel);

            });
        }

        private void SaveButtonCommandExecute(string args)
        {
            foreach (var aev in Inverters)
            {
                _configService.AppendConfig(aev.SerialNumber, aev.Alias + splitChar + aev.Exclude, ConfigTypes.Data, true);
            }
            _configService.CommitConfig(ConfigTypes.Data);
            MessageBox.Show("Success! \nAll changes will be appended on next inverter update or application restart");


            Application.Current.Windows[1].Close();
        }

        private bool SaveButtonCommandCanExecute(string args)
        {
            return Inverters.Count > 0;
        }

        private void CloseButtonCommandExecute(string args)
        {
            Application.Current.Windows[1].Close();
        }

        private void RaisePropertyChanged(string propertyName)
        {
            var handlers = PropertyChanged;

            handlers(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}