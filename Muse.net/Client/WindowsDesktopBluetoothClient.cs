using Muse.Net.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;

namespace Muse.Net.Client
{
    public class WindowsDesktopBluetoothClient<GattCharacteristicKeyType> : IBluetoothClient<GattCharacteristicKeyType, GattCharacteristic>
    {
        private BluetoothLEDevice _device;
        private GattDeviceService _service;
        private Dictionary<GattCharacteristicKeyType, GattCharacteristic> _charcteristics = new Dictionary<GattCharacteristicKeyType, GattCharacteristic>();
        private IList<GattCharacteristicKeyType> _subscriptions = new List<GattCharacteristicKeyType>();

        public string Name { get; private set; }
        public ulong Address { get; private set; }
        public bool Connected { get; private set; } = false;
        public IReadOnlyDictionary<GattCharacteristicKeyType, GattCharacteristic> Characteristics => _charcteristics;

        public Action<GattCharacteristicKeyType, byte[]> OnGattValueChanged { get; set; }

        public async Task<bool> Connect(
            ulong deviceAddress,
            Guid service,
            params KeyValuePair<GattCharacteristicKeyType, Guid>[] characteristics)
        {
            Address = deviceAddress;
            _device = await BluetoothLEDevice.FromBluetoothAddressAsync(Address);
            if (_device is null)
            {
                return false;
            }

            Name = _device.Name;
            _service = _device.GetGattService(service);
            if (_service is null)
            {
                return false;
            }

            foreach (var curCharacteristic in characteristics)
            {
                var characteristic = _service.GetCharacteristics(curCharacteristic.Value).FirstOrDefault();
                _charcteristics.Add(
                    curCharacteristic.Key,
                    characteristic);
            }

            Connected = true;
            return true;
        }

        public virtual Task Disconnect()
        {
            _charcteristics.Clear();
            _service.Dispose();
            _service = null;
            _device.Dispose();
            _service = null;
            Connected = false;

            return Task.CompletedTask;
        }

        public async Task<bool> SubscribeToChannel(GattCharacteristicKeyType characteristicKey)
        {
            var characteristic = Characteristics[characteristicKey];
            var status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            var ok = (status == GattCommunicationStatus.Success);
            if (ok)
            {
                characteristic.ValueChanged += GattValueChanged;
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
            var characteristic = Characteristics[characteristicKey];
            var status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
            var ok = (status == GattCommunicationStatus.Success);
            if (ok)
            {
                characteristic.ValueChanged -= GattValueChanged;
                _subscriptions.Remove(characteristicKey);
            }

            return ok;
        }

        private async Task<bool> SubscribeEvent(
            GattCharacteristicKeyType characteristicKey,
            TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> handler)
        {
            var characteristic = Characteristics[characteristicKey];
            var descriptor = await characteristic.ReadClientCharacteristicConfigurationDescriptorAsync();
            bool alreadyOn = (descriptor.ClientCharacteristicConfigurationDescriptor == GattClientCharacteristicConfigurationDescriptorValue.Notify);
            bool ok;
            if (!alreadyOn)
            {
                var status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                ok = status == GattCommunicationStatus.Success;
            }
            else
            {
                ok = true;
            }

            if (ok)
            {
                characteristic.ValueChanged += handler;
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
                var characteristic = Characteristics[characteristicKey];
                characteristic.ValueChanged -= handler;
                if (!keepOn)
                    await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
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
            GattCharacteristicKeyType characteristicKeyType = Characteristics.SingleOrDefault(x => x.Value == sender).Key;
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
