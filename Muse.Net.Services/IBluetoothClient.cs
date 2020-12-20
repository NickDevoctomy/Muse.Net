using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Muse.Net.Services
{
    public interface IBluetoothClient<GattCharacteristicKeyType, GattCharacteristicType>
    {
        string Name { get; }
        ulong Address { get; }
        bool Connected { get; }
        IReadOnlyDictionary<GattCharacteristicKeyType, GattCharacteristicType> Characteristics { get; }
        Action<GattCharacteristicKeyType, byte[]> OnGattValueChanged { get; set; }

        Task<bool> Connect(
            ulong deviceAddress,
            Guid service,
            params KeyValuePair<GattCharacteristicKeyType, Guid>[] characteristics);

        Task Disconnect();

        Task<bool> SubscribeToChannel(GattCharacteristicKeyType characteristicKey);

        Task<bool> UnsubscribeFromChannel(GattCharacteristicKeyType characteristicKey);

        Task<byte[]> SingleChannelEventAsync(GattCharacteristicKeyType characteristicKey);

        Task UnsubscribeAll();
    }
}
