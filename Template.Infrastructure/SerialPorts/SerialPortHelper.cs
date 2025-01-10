using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Infrastructure.SerialPorts
{
    public sealed class SerialPortHelper
    {
        public static IReadOnlyList<string> GetActiveSerialPorts()
        {
            return SerialPort.GetPortNames().Distinct().OrderBy(x => int.Parse(x.Replace("COM", string.Empty))).ToList();
        }

        public static bool IsConnectablePort(string portName)
        {
            var ports = SerialPort.GetPortNames();
            foreach (var port in ports)
            {
                if (port == portName)
                {
                    return true;
                }
            }
            return false;
        }

        public static void ClosePort(ref SerialPort port)
        {
            try
            {
                if (port.IsOpen)
                {
                    port.Close();
                }
                if (port != null)
                {
                    port.Dispose();
                }
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
