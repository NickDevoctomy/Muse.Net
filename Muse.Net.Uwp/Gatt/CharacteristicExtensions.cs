using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Harthoorn.MuseClient
{
    public static class CharacteristicExtensions
    {
        public async static Task<bool> WriteCommand(this GattCharacteristic control, string command)
        {
            var buffer = Language.EncodeCommand(command);
            var status = await control.WriteValueAsync(buffer, GattWriteOption.WriteWithoutResponse).AsTask();
            return (status == GattCommunicationStatus.Success);
        }
    }
}
