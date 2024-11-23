/*
  Application Name:     Unitgate  
  Created by:           Allan Wamming Lie
  Created:              2014-2025
  Company:              Lie Consulting
  E-mail:               unitgate@allanlie.dk 
  Web:                  unitgate.eu
 */


using System.Text.RegularExpressions;
using System.Windows;
using UnitGate.Helpers;
using UnitGate.Enums;
using UnitGate.Service;

namespace UnitGate.ViewModel
{
    internal class StaticIPViewModel
    {
        private readonly DelegateCommand<string> _saveButtonCommand;
        private readonly DelegateCommand<string> _closeButtonCommand;
        private ConfigService _configService;

        public string StaticIP { get; set; }


        public StaticIPViewModel()
        {
            _saveButtonCommand = new DelegateCommand<string>(SaveButtonCommandExecute);
            _closeButtonCommand = new DelegateCommand<string>(CloseButtonCommandExecute);


            _configService = ConfigService.Instance;
            StaticIP = _configService.GetConfig("IpAddress", ConfigTypes.General) ?? "";

        }

        public DelegateCommand<string> CloseButtonCommand
        {
            get { return _closeButtonCommand; }
        }

        public DelegateCommand<string> SaveButtonCommand
        {
            get { return _saveButtonCommand; }
        }



        private void SaveButtonCommandExecute(string args)
        {
            if (!Regex.Match(StaticIP, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b").Success)
            {
                MessageBox.Show("The entered IP is not valid! \nPlease enter a valid IP");
                return;
            }

            _configService.AppendConfig("IpAddress", StaticIP, ConfigTypes.General);
            _configService.CommitConfig(ConfigTypes.General);


            MessageBox.Show(string.Format("Ip Address: {0} Saved! \nPlease restart the application for the changes to apply", StaticIP));

            Application.Current.Windows[1].Close();

        }

        private void CloseButtonCommandExecute(string args)
        {
            Application.Current.Windows[1].Close();
        }




    }
}
