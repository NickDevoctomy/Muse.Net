﻿using Muse.Net.Services;
using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;

namespace Muse.Net.Uwp.Client
{
    public class UwpMuseDeviceDiscoveryService : IMuseDeviceDiscoveryService
    {
        public Task GetMuseDevices(Action<string, string> foundCallback)
        {
            var bleWatcher = new BluetoothLEAdvertisementWatcher
            {
                ScanningMode = BluetoothLEScanningMode.Active
            };
            var count = 0;
            var tcs = new TaskCompletionSource<int>();

            bleWatcher.Received += (w, args) =>
            {
                if (args.Advertisement.LocalName.IndexOf("Muse") < 0)
                {
                    return;
                }

                count = +1;
                foundCallback(
                    args.Advertisement.LocalName,
                    args.BluetoothAddress.ToString());
            };
            bleWatcher.Stopped += (w, args) =>
            {
                tcs.TrySetResult(count);
            };

            bleWatcher.Start();
            return tcs.Task;
        }
    }
}