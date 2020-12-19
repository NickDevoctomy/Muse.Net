using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Threading.Tasks;
using System;

namespace Muse.Net.Extensions
{
    public static class CharacteristicExtensions
    {
        public async static Task<bool> WriteCommand(this GattCharacteristic control, string command)
        {
            var buffer = command.EncodeCommandAsBuffer();
            var status = await control.WriteValueAsync(buffer, GattWriteOption.WriteWithoutResponse).AsTask();
            return (status == GattCommunicationStatus.Success);
        }
    }
}