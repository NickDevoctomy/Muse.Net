using Muse.Net.Services;
using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace Muse.Net.Client
{
    public class WindowsDesktopMuseDeviceDiscoveryService : IMuseDeviceDiscoveryService
    {
        public Task GetMuseDevices(Action<string, string> foundCallback)
        {
            string query = BluetoothLEDevice.GetDeviceSelectorFromPairingState(true);
            var devWatch = DeviceInformation.CreateWatcher(query);
            var count = 0;
            var tcs = new TaskCompletionSource<int>();

            devWatch.Added += (DeviceWatcher sender, DeviceInformation args) =>
            {
                if (args.Name.IndexOf("Muse") < 0)
                {
                    return;
                }

                count += 1;
                foundCallback(
                    args.Name,
                    args.Id);
            };
            devWatch.EnumerationCompleted += (DeviceWatcher sender, object args) =>
            {
                tcs.TrySetResult(count);
                devWatch.Stop();
            };
            devWatch.Start();
            return tcs.Task;
        }
    }
}
