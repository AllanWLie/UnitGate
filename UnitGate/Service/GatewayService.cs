/*
  Application Name:     Unitgate  
  Created by:           Allan Wamming Lie
  Created:              2014-2016
  Company:              Lie Development
  E-mail:               unitgate@allanlie.dk 
  Web:                  unitgate.eu
 */

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Shared.Models;
using Shared.Services;

namespace UnitGate.Service
{
    class GatewayService
    {

        private readonly string _gatewayIp;
        private ZigbitService _zigbitService;


        public GatewayService(string gatewayIp)
        {
            _gatewayIp = gatewayIp;
            _zigbitService = new ZigbitService();
        }

        public async void StartDataCollection()
        {
            try
            {
                while (true)
                {
                    await Task.Run(() => GetGatewayData());
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                ErrorHandlingService.PersistError("GatewayService - StartDataCollection", ex);
               
            }
        }

        private void GetGatewayData()
        {
            var uri = new Uri("http://" + _gatewayIp + "/ajax.xml");

            using (var webClient = new WebClient())
            {
                webClient.DownloadStringAsync(uri);
                webClient.DownloadStringCompleted += webClient_DownloadStringCompleted;
            }
        }

        private void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e != null && e.Error == null)
                {
                    var data = e.Result;
                    XmlDocument _doc = new XmlDocument();
                    _doc.LoadXml(data);
                   
                    if (_doc.GetElementsByTagName("zigbeeData").Count > 0)
                    {
                        string zigbeeData = _doc.GetElementsByTagName("zigbeeData")[0].InnerText;


                        Zigbit zigbit = _zigbitService.CollectData(zigbeeData);
                        if (zigbit != null)
                        {
                            UpdateZigbit(zigbit);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandlingService.PersistError("GatewayService - webClient_DownloadStringCompleted", ex);
                
            }
        }
        public event Action<Zigbit> OnZigbitUpdated;
        private void UpdateZigbit(Zigbit data)
        {
            if (OnZigbitUpdated != null)
            {
                OnZigbitUpdated(data);
            }
        }
    
    }
}
