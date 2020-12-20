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
    public class BluetoothClient<CharacteristicKeyType>
    {
        private BluetoothLEDevice _device;
        private GattDeviceService _service;
        private Dictionary<CharacteristicKeyType, GattCharacteristic> _charcteristics = new Dictionary<CharacteristicKeyType, GattCharacteristic>();

        public string Name { get; private set; }
        public ulong Address { get; private set; }
        public bool Connected { get; private set; } = false;
        public IReadOnlyDictionary<CharacteristicKeyType, GattCharacteristic> Characteristics => _charcteristics;
        public IList<CharacteristicKeyType> Subscriptions { get; private set; } = new List<CharacteristicKeyType>();

#if WINDOWS_UWP
        public async Task<bool> Connect(
            ulong deviceAddress,
            Guid service,
            params KeyValuePair<CharacteristicKeyType, Guid>[] characteristics)
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
                _charcteristics.Add(
                    curCharacteristic.Key,
                    characteristic);
            }

            Connected = true;
            return true;
        }
#else
        public async Task<bool> Connect(
            ulong deviceAddress,
            Guid service,
            params KeyValuePair<CharacteristicKeyType, Guid>[] characteristics)
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

            foreach(var curCharacteristic in characteristics)
            {
                var characteristic = _service.GetCharacteristics(curCharacteristic.Value).FirstOrDefault();
                _charcteristics.Add(
                    curCharacteristic.Key,
                    characteristic);
            }

            Connected = true;
            return true;
        }
#endif

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

        public virtual async Task<bool> SubscribeToChannel(CharacteristicKeyType characteristicKey)
        {
            var characteristic = Characteristics[characteristicKey];
            var status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            var ok = (status == GattCommunicationStatus.Success);
            if (ok)
            {
                characteristic.ValueChanged += GattValueChanged;
                Subscriptions.Add(characteristicKey);
            }

            return ok;
        }

        public virtual async Task<bool> UnsubscribeFromChannel(CharacteristicKeyType characteristicKey)
        {
            var characteristic = Characteristics[characteristicKey];
            var status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
            var ok = (status == GattCommunicationStatus.Success);
            if (ok)
            {
                characteristic.ValueChanged -= GattValueChanged;
                Subscriptions.Remove(characteristicKey);
            }

            return ok;
        }

        protected async Task<bool> SubscribeEvent(
            CharacteristicKeyType characteristicKey,
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

        protected async Task UnsubscribeEvent(
            CharacteristicKeyType characteristicKey,
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

        protected async Task<byte[]> SingleChannelEventAsync(CharacteristicKeyType characteristicKey)
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
            CharacteristicKeyType characteristicKeyType = Characteristics.SingleOrDefault(x => x.Value == sender).Key;
            var data = args.CharacteristicValue.ToArray();
            OnGattValueChanged(
                characteristicKeyType,
                data);
        }

        protected virtual void OnGattValueChanged(
            CharacteristicKeyType characteristicKeyType,
            byte[] data)
        {
        }
    }
}
