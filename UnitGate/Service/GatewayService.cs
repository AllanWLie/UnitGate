/*
  Application Name:     Unitgate  
  Created by:           Allan Wamming Lie
  Created:              2014-2025
  Company:              Lie Consulting
  E-mail:               unitgate@allanlie.dk 
  Web:                  unitgate.eu
 */

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using UnitGate.Models;
using UnitGate.Services;

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

    private async void GetGatewayData()
    {
      var uri = new Uri("http://" + _gatewayIp + "/ajax.xml");

      using (HttpClient httpClient = new HttpClient())
      {
        try
        {
          HttpResponseMessage response = await httpClient.GetAsync(uri);
          if (response != null && response.IsSuccessStatusCode)
          {
            string responseData = await response.Content.ReadAsStringAsync();
          }
        }
        catch (Exception)
        {
          throw;
        }

      }
    }

    private void ProcessGatewayData(string data)
    {
      try
      {
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
      catch (Exception ex)
      {
        ErrorHandlingService.PersistError("GatewayService - ProcessGatewayData", ex);

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
