﻿using Muse.Net.Extensions;
using Muse.Net.Models;
using Muse.Net.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
#if WINDOWS_UWP
    using Windows.Devices.Bluetooth.Advertisement;
#endif
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;

namespace Muse.Net.Client
{
    public class MuseClient
    {
        public event Action<Telemetry> NotifyTelemetry;
        public event Action<Accelerometer> NotifyAccelerometer;
        public event Action<Gyroscope> NotifyGyroscope;
        public event EventHandler<MuseClientNotifyEegEventArgs> NotifyEeg;

        public string Name { get; private set; }
        public ulong Address { get; private set; }
        public bool Connected { get; private set; } = false;
        public IList<Channel> Subscriptions { get; private set; } = new List<Channel>();

        private BluetoothLEDevice device;
        private GattDeviceService service;
        private GattCharacteristic ch_control;
        private GattCharacteristic ch_accelerometer;
        private GattCharacteristic ch_gyroscope;
        private GattCharacteristic ch_telemetry;
        private GattCharacteristic ch_EEG_TP9;
        private GattCharacteristic ch_EEG_AF7;
        private GattCharacteristic ch_EEG_AF8;
        private GattCharacteristic ch_EEG_TP10;
        private GattCharacteristic ch_EEG_AUX;

        public MuseClient()
        {
        }

        public async Task<bool> Connect()
        {
            ulong? bluetoothDeviceId = await FindPairedMuseDevice();
            if (bluetoothDeviceId == null)
                return false; // No muse found
            
            return await Connect(bluetoothDeviceId.Value);
        }

#if WINDOWS_UWP

        public async Task<bool> Connect(ulong deviceAddress)
        {
            this.Address = deviceAddress;
            device = await BluetoothLEDevice.FromBluetoothAddressAsync(this.Address);

            if (device is null) return false;

            var allServicesResult = await device.GetGattServicesAsync();
            if (allServicesResult.Status != GattCommunicationStatus.Success)
            {
                return false;
            }

            service = allServicesResult.Services.SingleOrDefault(x => x.Uuid == MuseGuid.PRIMARY_SERVICE);
            if (service is null) return false;

            var allCharacteristicsResult = await service.GetCharacteristicsAsync();
            if (allCharacteristicsResult.Status != GattCommunicationStatus.Success)
            {
                return false;
            }

            var allCharacteristics = allCharacteristicsResult.Characteristics.ToList();

            ch_control = allCharacteristics.SingleOrDefault(x => x.Uuid == MuseGuid.CONTROL);
            ch_accelerometer = allCharacteristics.SingleOrDefault(x => x.Uuid == MuseGuid.ACELEROMETER);
            ch_gyroscope = allCharacteristics.SingleOrDefault(x => x.Uuid == MuseGuid.GYROSCOPE);
            ch_telemetry = allCharacteristics.SingleOrDefault(x => x.Uuid == MuseGuid.TELEMETRY);

            ch_EEG_TP9 = allCharacteristics.SingleOrDefault(x => x.Uuid == MuseGuid.EEG_TP9);
            ch_EEG_AF7 = allCharacteristics.SingleOrDefault(x => x.Uuid == MuseGuid.EEG_AF7);
            ch_EEG_AF8 = allCharacteristics.SingleOrDefault(x => x.Uuid == MuseGuid.EEG_AF8);
            ch_EEG_TP10 = allCharacteristics.SingleOrDefault(x => x.Uuid == MuseGuid.EEG_TP10);
            ch_EEG_AUX = allCharacteristics.SingleOrDefault(x => x.Uuid == MuseGuid.EEG_AUX);

            Connected = true;
            return true;
        }

        public static Task<ulong?> FindPairedMuseDevice()
        {
            var bleWatcher = new BluetoothLEAdvertisementWatcher
            {
                ScanningMode = BluetoothLEScanningMode.Active
            };
            var tcs = new TaskCompletionSource<ulong?>();

            bleWatcher.Received += (w, btAdv) =>
            {
                if (btAdv.Advertisement.LocalName.IndexOf("Muse") < 0)
                {
                    return;
                }

                tcs.TrySetResult(btAdv.BluetoothAddress);
            };

            bleWatcher.Start();
            return tcs.Task;
        }

#else

        public async Task<bool> Connect(ulong deviceAddress)
        {
            this.Address = deviceAddress;
            device = await BluetoothLEDevice.FromBluetoothAddressAsync(this.Address);

            if (device is null) return false;

            service = device.GetGattService(MuseGuid.PRIMARY_SERVICE);
            if (service is null) return false;

            ch_control = service.GetCharacteristics(MuseGuid.CONTROL).FirstOrDefault();
            ch_accelerometer = service.GetCharacteristics(MuseGuid.ACELEROMETER).FirstOrDefault();
            ch_gyroscope = service.GetCharacteristics(MuseGuid.GYROSCOPE).FirstOrDefault();
            ch_telemetry = service.GetCharacteristics(MuseGuid.TELEMETRY).FirstOrDefault();
            ch_EEG_TP9 = service.GetCharacteristics(MuseGuid.EEG_TP9).FirstOrDefault();
            ch_EEG_AF7 = service.GetCharacteristics(MuseGuid.EEG_AF7).FirstOrDefault();
            ch_EEG_AF8 = service.GetCharacteristics(MuseGuid.EEG_AF8).FirstOrDefault();
            ch_EEG_TP10 = service.GetCharacteristics(MuseGuid.EEG_TP10).FirstOrDefault();
            ch_EEG_AUX = service.GetCharacteristics(MuseGuid.EEG_AUX).FirstOrDefault();

            Connected = true;
            return true;
        }

        public static Task<ulong?> FindPairedMuseDevice()
        {
            string query = BluetoothLEDevice.GetDeviceSelectorFromPairingState(true);
            var devWatch = DeviceInformation.CreateWatcher(query);
            var tcs = new TaskCompletionSource<ulong?>();
   
            devWatch.Added += async (DeviceWatcher sender, DeviceInformation args) =>
            {
                if (args.Name.IndexOf("Muse") < 0)
                    return;
                devWatch.Stop();

                var device = await BluetoothLEDevice.FromIdAsync(args.Id);
                tcs.TrySetResult(device.BluetoothAddress);
            };
            devWatch.EnumerationCompleted += (DeviceWatcher sender, object args) =>
            {
                tcs.TrySetResult(null);
                devWatch.Stop();
            };
            devWatch.Start();
            return tcs.Task;
        }

#endif

        public async Task Disconnect()
        {
            await UnsubscribeAll();
        }

        public async Task Subscribe(params Channel[] channels)
        {
            foreach (var channel in channels)
            {
                await SubscribeToChannel(channel);
            }
        }

        public async Task Resume()
        {
            await ch_control.WriteCommand(MuseCommand.RESUME);
        }

        public async Task Start()
        {
            await ch_control.WriteCommand(MuseCommand.START);

        }

        public async Task Pause()
        {
            await ch_control.WriteCommand(MuseCommand.PAUSE);
        }

        public async Task UnsubscribeAll()
        {
            foreach (var channel in Subscriptions)
            {
                await UnsubscribeFromChannel(channel);
            }
        }

        public async Task<bool> SubscribeToChannel(Channel channel)
        {
            var characteristic = GetCharacteristic(channel);
            var status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            var ok = (status == GattCommunicationStatus.Success);

            if (ok)
            {
                characteristic.ValueChanged += Notify;
                Subscriptions.Add(channel);
            }

            return ok;
        }

        public async Task<bool> UnsubscribeFromChannel(Channel channel)
        {
            var characteristic = GetCharacteristic(channel);
            var status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
            var ok = (status == GattCommunicationStatus.Success);

            if (ok)
            {
                characteristic.ValueChanged -= Notify;
                Subscriptions.Remove(channel);
            }

            return ok;
        }

        private void Notify(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var bytes = args.CharacteristicValue.ToArray();
            var channel = GetChannel(sender);
            switch (channel)
            {
                case Channel.Telemetry: TriggerNotifyTelemetry(bytes); break;
                case Channel.Accelerometer: TriggerNotifyAccelerometer(bytes); break;
                case Channel.Gyroscope: TriggerNotifyGyroscope(bytes); break;
                case Channel.EEG_AF7:
                case Channel.EEG_AF8:
                case Channel.EEG_TP9:
                case Channel.EEG_TP10:
                case Channel.EEG_AUX: TriggerNotifyEeg(channel, bytes); break;

            }
        }

        private async Task<bool> SubscribeEvent(Channel channel, TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> handler)
        {
            var characteristic = GetCharacteristic(channel);
            var descriptor = await characteristic.ReadClientCharacteristicConfigurationDescriptorAsync();
            bool alreadyOn = (descriptor.ClientCharacteristicConfigurationDescriptor == GattClientCharacteristicConfigurationDescriptorValue.Notify);
            bool ok;

            if (!alreadyOn)
            {
                var status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                ok = status == GattCommunicationStatus.Success;
            }
            else ok = true;

            if (ok) characteristic.ValueChanged += handler;

            return alreadyOn;
        }

        private async Task UnsubscribeEvent(Channel channel, TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> handler, bool keepOn)
        {
            if (handler != null)
            {
                var characteristic = GetCharacteristic(channel);
                characteristic.ValueChanged -= handler;
                if (!keepOn)
                    await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                
            }
        }

        public async Task<byte[]> SingleChannelEventAsync(Channel channel)
        {
            var completion = new TaskCompletionSource<byte[]>();

            bool keep = await SubscribeEvent(channel, NotifyTaskCompletion);

            var result = await completion.Task;

            await UnsubscribeEvent(channel, NotifyTaskCompletion, keep);

            return result;


            void NotifyTaskCompletion(GattCharacteristic sender, GattValueChangedEventArgs args)
            {
                var bytes = args.CharacteristicValue.ToArray();
                completion.SetResult(bytes);
            }
        }

        public async Task<Telemetry> ReadTelemetryAsync()
        {
            var bytes = await SingleChannelEventAsync(Channel.Telemetry);
            if (bytes != null)
            {
                return Parse.Telemetry(bytes);
            }
            else return null;
        }

        private void TriggerNotifyEeg(Channel channel, ReadOnlySpan<byte> bytes)
        { 
            var eeg = Parse.Encefalogram(bytes);
            NotifyEeg?.Invoke(this, new MuseClientNotifyEegEventArgs { Channel = channel, Encefalogram = eeg });
        }

        private void TriggerNotifyTelemetry(ReadOnlySpan<byte> bytes)
        {
            var telemetry = Parse.Telemetry(bytes);
            NotifyTelemetry?.Invoke(telemetry);
        }

        private void TriggerNotifyAccelerometer(ReadOnlySpan<byte> bytes)
        {
            var accelerometer = Parse.Accelerometer(bytes);
            NotifyAccelerometer?.Invoke(accelerometer);
        }

        private void TriggerNotifyGyroscope(ReadOnlySpan<byte> bytes)
        {
            var gyroscope = Parse.Gyroscope(bytes);
            NotifyGyroscope?.Invoke(gyroscope);
        }

        private GattCharacteristic GetCharacteristic(Channel channel)
        {
            switch (channel)
            {
                case Channel.Accelerometer: return ch_accelerometer;
                case Channel.Control: return ch_control;
                case Channel.Gyroscope: return ch_gyroscope;
                case Channel.Telemetry: return ch_telemetry;
                case Channel.EEG_AF7: return ch_EEG_AF7;
                case Channel.EEG_AF8: return ch_EEG_AF8;
                case Channel.EEG_TP9: return ch_EEG_TP9;
                case Channel.EEG_TP10: return ch_EEG_TP10;
                case Channel.EEG_AUX: return ch_EEG_AUX;
                default: return null;
            }
        }

        private Channel GetChannel(GattCharacteristic ch) 
        {
            if (ch == ch_control) return Channel.Control;
            if (ch == ch_accelerometer) return Channel.Accelerometer;
            if (ch == ch_telemetry) return Channel.Telemetry;
            if (ch == ch_gyroscope) return Channel.Gyroscope;
            if (ch == ch_EEG_AF7) return Channel.EEG_AF7;
            if (ch == ch_EEG_AF8) return Channel.EEG_AF8;
            if (ch == ch_EEG_TP9) return Channel.EEG_TP9;
            if (ch == ch_EEG_TP10) return Channel.EEG_TP10;
            if (ch == ch_EEG_AUX) return Channel.EEG_AUX;

            return Channel.None;        
        }
    }

}