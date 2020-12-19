using System;
using System.Threading;
using System.Threading.Tasks;

namespace Muse.Net.Services
{
    public interface IMuseDeviceDiscoveryService
    {
        Task GetMuseDevices(Action<string, string> foundCallback);
    }
}
