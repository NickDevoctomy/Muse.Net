using Muse.Net.Client;
using Muse.Net.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;

namespace Muse.Net.Uwp.Client
{
    public class UwpBluetoothClient<GattCharacteristicKeyType> : IBluetoothClient<GattCharacteristicKeyType, IGattCharacteristic>
    {
        private BluetoothLEDevice _device;
        private GattDeviceService _service;
        private Dictionary<GattCharacteristicKeyType, IGattCharacteristic> _characteristics = new Dictionary<GattCharacteristicKeyType, IGattCharacteristic>();
        private IList<GattCharacteristicKeyType> _subscriptions = new List<GattCharacteristicKeyType>();

        public string Name { get; private set; }
        public ulong Address { get; private set; }
        public bool Connected { get; private set; } = false;
        public IReadOnlyDictionary<GattCharacteristicKeyType, IGattCharacteristic> Characteristics => _characteristics;

        public Action<GattCharacteristicKeyType, byte[]> OnGattValueChanged { get; set; }

        public async Task<bool> Connect(
            ulong deviceAddress,
            Guid service,
            params KeyValuePair<GattCharacteristicKeyType, Guid>[] characteristics)
        {
            Address = deviceAddress;
            _device = await BluetoothLEDevice.FromBluetoothAddressAsync(this.Address);

            if (_device is null)
            {
                return false;
            }

            var allServicesResult = await _device.GetGattServicesAsync();
            if (allServicesResult.Status != GattCommunicationStatus.Success)
            {
                return false;
            }

            _service = allServicesResult.Services.SingleOrDefault(x => x.Uuid == MuseGuid.PRIMARY_SERVICE);
            if (_service is null)
            {
                return false;
            }

            var allCharacteristicsResult = await _service.GetCharacteristicsAsync();
            if (allCharacteristicsResult.Status != GattCommunicationStatus.Success)
            {
                return false;
            }

            var allCharacteristics = allCharacteristicsResult.Characteristics.ToList();
            foreach (var curCharacteristic in characteristics)
            {
                var characteristic = allCharacteristics.SingleOrDefault(x => x.Uuid == curCharacteristic.Value);
                _characteristics.Add(
                    curCharacteristic.Key,
                    new WrappedGatCharacteristic(characteristic));
            }

            Connected = true;
            return true;
        }

        public virtual Task Disconnect()
        {
            _characteristics.Clear();
            _service.Dispose();
            _service = null;
            _device.Dispose();
            _service = null;
            Connected = false;

            return Task.CompletedTask;
        }

        public async Task<bool> SubscribeToChannel(GattCharacteristicKeyType characteristicKey)
        {
            var characteristic = (WrappedGatCharacteristic)Characteristics[characteristicKey];
            var status = await characteristic.Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            var ok = (status == GattCommunicationStatus.Success);
            if (ok)
            {
                characteristic.Characteristic.ValueChanged += GattValueChanged;
                _subscriptions.Add(characteristicKey);
            }

            return ok;
        }

        public async Task UnsubscribeAll()
        {
            foreach (var channel in _subscriptions)
            {
                await UnsubscribeFromChannel(channel);
            }
        }

        public async Task<bool> UnsubscribeFromChannel(GattCharacteristicKeyType characteristicKey)
        {
            var characteristic = (WrappedGatCharacteristic)Characteristics[characteristicKey];
            var status = await characteristic.Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
            var ok = (status == GattCommunicationStatus.Success);
            if (ok)
            {
                characteristic.Characteristic.ValueChanged -= GattValueChanged;
                _subscriptions.Remove(characteristicKey);
            }

            return ok;
        }

        private async Task<bool> SubscribeEvent(
            GattCharacteristicKeyType characteristicKey,
            TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> handler)
        {
            var characteristic = (WrappedGatCharacteristic)Characteristics[characteristicKey];
            var descriptor = await characteristic.Characteristic.ReadClientCharacteristicConfigurationDescriptorAsync();
            bool alreadyOn = (descriptor.ClientCharacteristicConfigurationDescriptor == GattClientCharacteristicConfigurationDescriptorValue.Notify);
            bool ok;
            if (!alreadyOn)
            {
                var status = await characteristic.Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                ok = status == GattCommunicationStatus.Success;
            }
            else
            {
                ok = true;
            }

            if (ok)
            {
                characteristic.Characteristic.ValueChanged += handler;
            }

            return alreadyOn;
        }

        private async Task UnsubscribeEvent(
            GattCharacteristicKeyType characteristicKey,
            TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> handler,
            bool keepOn)
        {
            if (handler != null)
            {
                var characteristic = (WrappedGatCharacteristic)Characteristics[characteristicKey];
                characteristic.Characteristic.ValueChanged -= handler;
                if (!keepOn)
                    await characteristic.Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
            }
        }

        public async Task<byte[]> SingleChannelEventAsync(GattCharacteristicKeyType characteristicKey)
        {
            var completion = new TaskCompletionSource<byte[]>();
            bool keep = await SubscribeEvent(characteristicKey, NotifyTaskCompletion);
            var result = await completion.Task;
            await UnsubscribeEvent(characteristicKey, NotifyTaskCompletion, keep);
            return result;

            // !!! I don't like this being here but will leave it for now
            void NotifyTaskCompletion(
                GattCharacteristic sender,
                GattValueChangedEventArgs args)
            {
                var bytes = args.CharacteristicValue.ToArray();
                completion.SetResult(bytes);
            }
        }

        private void GattValueChanged(
            GattCharacteristic sender,
            GattValueChangedEventArgs args)
        {
            GattCharacteristicKeyType characteristicKeyType = Characteristics.SingleOrDefault(x => ((WrappedGatCharacteristic)x.Value).Characteristic == sender).Key;
            var data = args.CharacteristicValue.ToArray();
            if (OnGattValueChanged != null)
            {
                OnGattValueChanged(
                    characteristicKeyType,
                    data);
            }
        }
    }
}
