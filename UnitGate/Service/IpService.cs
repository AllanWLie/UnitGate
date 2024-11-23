/*
  Application Name:     Unitgate  
  Created by:           Allan Wamming Lie
  Created:              2014-2025
  Company:              Lie Consulting
  E-mail:               unitgate@allanlie.dk 
  Web:                  unitgate.eu
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace UnitGate.Service
{
  internal class IpService
  {

    public async Task<bool> ValidateIpAsync(string gatewayIp)
    {
      return await CheckXmlFileExists(gatewayIp);
    }

    public async Task<string> GetGatewayIp()
    {

      int maxConcurrency = 10; // Adjust as needed for your environment
      var localSubnets = GetLocalSubnets(); // Get all subnet ranges
      var cts = new CancellationTokenSource();
      var semaphore = new SemaphoreSlim(maxConcurrency);

      string foundIp = null;

      try
      {
        var tasks = localSubnets.SelectMany(subnet => GenerateIPs(subnet))
            .Select(async ip =>
            {
              await semaphore.WaitAsync(cts.Token);
              try
              {
                if (cts.IsCancellationRequested) return; // Exit early if already cancelled


                Console.WriteLine($"Trying Ip {ip}...");

                if (await CheckXmlFileExists(ip))
                {
                  Console.WriteLine($"XML found at {ip}");
                  foundIp = ip;
                  cts.Cancel(); // Signal all tasks to stop
                }
              }
              catch (OperationCanceledException)
              {
                // Handle cancellation
              }
              finally
              {
                semaphore.Release();
              }
            }).ToList();

        // Wait for tasks to complete or be canceled
        await Task.WhenAll(tasks);
      }
      finally
      {
        semaphore.Dispose();
        cts.Dispose();
      }

      if (foundIp != null)
      {
        Console.WriteLine($"Found XML at IP: {foundIp}");
      }
      else
      {
        Console.WriteLine("XML file not found on any IP address.");
      }

      return foundIp;
    }

    private List<(string baseIp, int prefixLength)> GetLocalSubnets()
    {
      var subnets = new List<(string BaseIp, int PrefixLength)>();

      var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces().Where(x => 
        x.OperationalStatus == OperationalStatus.Up &&
        (x.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
        x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211));
      
      foreach (var networkInterface in networkInterfaces)
      {
        var ipProperties = networkInterface.GetIPProperties();
        if (ipProperties.GatewayAddresses.Count <= 0)
        {
          continue;
        }
        
        foreach (var unicast in ipProperties.UnicastAddresses)
        {
          if (unicast.Address.AddressFamily == AddressFamily.InterNetwork)
          {
            string baseIp = unicast.Address.ToString();
            int prefixLength = unicast.PrefixLength; // Subnet prefix length
            subnets.Add((baseIp, prefixLength));
          }
        }
      }

      return subnets;
    }

    private IEnumerable<string> GenerateIPs((string BaseIp, int PrefixLength) subnet)
    {
      var parts = subnet.BaseIp.Split('.').Select(int.Parse).ToArray();
      int subnetSize = 1 << (32 - subnet.PrefixLength); // Calculate number of addresses in the subnet
      int baseIpNumeric = (parts[0] << 24) | (parts[1] << 16) | (parts[2] << 8) | parts[3];

      for (int i = 1; i < subnetSize - 1; i++) // Skip network and broadcast addresses
      {
        int ipNumeric = baseIpNumeric + i;
        yield return $"{(ipNumeric >> 24) & 0xFF}.{(ipNumeric >> 16) & 0xFF}.{(ipNumeric >> 8) & 0xFF}.{ipNumeric & 0xFF}";
      }
    }

    private async Task<bool> CheckXmlFileExists(string ip)
    {
      string targetPath = "/ajax.xml";
      int port = 80;
      string url = $"http://{ip}:{port}{targetPath}";
      try
      {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
        var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

        if (response.IsSuccessStatusCode && response.Content.Headers.ContentType?.MediaType == "application/xml")
        {
          return true;
        }
      }
      catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
      {
        // Explicitly handle timeout exceptions
        Console.WriteLine($"Request to {url} timed out.");
      }
      catch (HttpRequestException ex)
      {
        // Log other HTTP errors for debugging
        Console.WriteLine($"HTTP error on {url}: {ex.Message}");
      }
      catch (Exception ex)
      {
        // Catch all other exceptions
        Console.WriteLine($"Unexpected error on {url}: {ex.Message}");
      }

      return false;
    }
  }
}
