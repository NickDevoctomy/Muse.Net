using Microsoft.Extensions.DependencyInjection;
using Muse.Net.Client;
using Muse.Net.Models.Enums;
using Muse.Net.Services;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Muse.Net.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMuseServices(this ServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IMuseDeviceDiscoveryService, WindowsDesktopMuseDeviceDiscoveryService>();
            serviceCollection.AddScoped<IMuseDataParserService, MuseDataParserService>();
            serviceCollection.AddScoped<IBluetoothClient<Channel, IGattCharacteristic>, WindowsDesktopBluetoothClient<Channel>>();
            serviceCollection.AddScoped<IMuseClient, MuseClient>();
        }
    }
}