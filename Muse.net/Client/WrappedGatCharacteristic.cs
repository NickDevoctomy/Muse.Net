using Muse.Net.Extensions;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Muse.Net.Client
{
    public class WrappedGatCharacteristic : IGattCharacteristic
    {
        public GattCharacteristic Characteristic { get; }

        public WrappedGatCharacteristic(GattCharacteristic characteristic)
        {
            Characteristic = characteristic;
        }

        public Task<bool> WriteCommand(string command)
        {
            return Characteristic.WriteCommand(command);
        }
    }
}
