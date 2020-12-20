using Microsoft.Extensions.DependencyInjection;
using Muse.Net.Client;
using Muse.Net.Models.Enums;
using Muse.Net.Services;
using Muse.Net.Uwp.Client;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Muse.Net.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMuseServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IMuseDeviceDiscoveryService, UwpMuseDeviceDiscoveryService>();
            serviceCollection.AddScoped<IMuseDataParserService, MuseDataParserService>();
            serviceCollection.AddScoped<IBluetoothClient<Channel, IGattCharacteristic>, UwpBluetoothClient<Channel>>();
            serviceCollection.AddScoped<IMuseClient, MuseClient>();
        }
    }
}
