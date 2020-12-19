using Muse.Net.Models;
using System;
using System.Threading.Tasks;

namespace Muse.Net.Services
{
    public interface IMuseDeviceDiscoveryService
    {
        Task<int> GetMuseDevices(Action<MuseDevice> foundCallback);
    }
}
