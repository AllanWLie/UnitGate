/*
  Application Name:     Unitgate  
  Created by:           Allan Wamming Lie
  Created:              2014-2016
  Company:              Lie Development
  E-mail:               unitgate@allanlie.dk 
  Web:                  unitgate.eu
 */

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace UnitGate.Service
{
    internal class IpService
    {
        private readonly string[] _localIps;
        private string _gatewayIp;

      

        public IpService()
        {
            _localIps = GetLocalIps().Split(',');
        }


        private string GetLocalIps()
        {
            var result = "";
            var localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var addr in localIPs)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (string.IsNullOrEmpty(result))
                        result = addr.ToString();
                    else
                        result = result + "," + addr;
                }
            }
            return result;
        }

        public async Task<string> ValidateIpAsync(string gatewayIp)
        {
            await Task.Run(() => PingService(gatewayIp, null));

            if (string.IsNullOrEmpty(_gatewayIp))
            {
                await GetGatewayIp();
            }

            return _gatewayIp;
        }

        public async Task<string> GetGatewayIp()
        {
            await Task.Run(() => PingServiceParallel());
            return _gatewayIp;
        }

        private void PingServiceParallel()
        {
            while (string.IsNullOrEmpty(_gatewayIp))
            {
                foreach (var localIp in _localIps)
                {
                        var ipTrim = localIp.Substring(0, localIp.LastIndexOf("."));
                        Parallel.For(1, 255, (i, parallelLoopState) => { PingService(ipTrim + "." + i,parallelLoopState); });
                        if (!string.IsNullOrEmpty(_gatewayIp))
                        {
                            return;
                        }
                }
                Thread.Sleep(5000);
     
            }
        }

        private void PingService(string ip, ParallelLoopState state)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            var reqUrl = "http://" + ip + "/ajax.xml";
            request = (HttpWebRequest) WebRequest.Create(reqUrl);
            request.Timeout = 20000;
            request.AllowAutoRedirect = false;
            Debug.WriteLine(reqUrl);

            var flag = -1;
            try
            {
                response = (HttpWebResponse) request.GetResponse();
                flag = 1;
            }
            catch
            {
                flag = -1;
            }

            if (flag == 1)
            {
                if (response != null && response.ContentType.Equals("text/xml"))
                {
                    _gatewayIp = ip;
                    if (state != null)
                    {
                        state.Stop();
                    }
                }
            }
        }
    }
}