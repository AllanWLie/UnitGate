/*
  Application Name:     Unitgate  
  Created by:           Allan Wamming Lie
  Created:              2014-2025
  Company:              Lie Consulting
  E-mail:               unitgate@allanlie.dk 
  Web:                  unitgate.eu
 */

using System.Windows;
using UnitGate.Helpers;

namespace UnitGate.ViewModel
{
    internal class AboutViewModel
    {
        private readonly DelegateCommand<string> _closeCommand;

        public AboutViewModel()
        {
            _closeCommand = new DelegateCommand<string>(CloseCommandExecute);
        }

        public DelegateCommand<string> CloseCommand
        {
            get { return _closeCommand; }
        }

        private void CloseCommandExecute(string args)
        {
            if (Application.Current.Windows.Count > 1)
            {
                Application.Current.Windows[1].Close();
            }
            else
            {
                Application.Current.Windows[0].Close();
            }
        }
    }
}