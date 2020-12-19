using Muse.Net.Models;
using Muse.Net.Services;
using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace Muse.Net.Client
{
    public class WindowsDesktopMuseDeviceDiscoveryService : IMuseDeviceDiscoveryService
    {
        public Task<int> GetMuseDevices(Action<MuseDevice> foundCallback)
        {
            string query = BluetoothLEDevice.GetDeviceSelectorFromPairingState(true);
            var devWatch = DeviceInformation.CreateWatcher(query);
            var count = 0;
            var tcs = new TaskCompletionSource<int>();

            devWatch.Added += async (DeviceWatcher sender, DeviceInformation args) =>
            {
                if (args.Name.IndexOf("Muse") < 0)
                {
                    return;
                }

                count += 1;
                var device = await BluetoothLEDevice.FromIdAsync(args.Id);
                foundCallback(
                    new MuseDevice
                    {
                        Name = args.Name,
                        Id = device.BluetoothAddress.ToString()
                    });
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
