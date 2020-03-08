using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Xin.Core.Utils
{
    public class NetworkUtil
    {
        public static Task<int> WakeUp(byte[] mac, int port = 7)
        {
            using (var client = new UdpClient())
            {
                client.Connect(IPAddress.Broadcast, port);
                var packet = new byte[17 * 6];
                for (var i = 0; i < 6; i++)
                    packet[i] = 0xFF;
                for (var i = 1; i <= 16; i++)
                for (var j = 0; j < 6; j++)
                    packet[i * 6 + j] = mac[j];
                return client.SendAsync(packet, packet.Length);
            }
        }


        public static Task<int> WakeUp(string mac, int port = 7)
        {
            return WakeUp(MacToArray(mac), port);
        }


        public static byte[] MacToArray(string mac)
        {
            if (string.IsNullOrEmpty(mac))
            {
                throw new ArgumentNullException(nameof(mac));
            }

            mac = mac.Trim().Replace("-", "").Replace(":", "").Replace(" ", "");
            var _mac = new byte[mac.Length / 2];
            for (var i = 0; i < _mac.Length; i++)
            {
                _mac[i] = Convert.ToByte(mac.Substring(i * 2, 2), 16);
            }

            return _mac;
        }
    }
}