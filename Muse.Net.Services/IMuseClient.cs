using Muse.Net.Models;
using Muse.Net.Models.Enums;
using System;
using System.Threading.Tasks;

namespace Muse.Net.Services
{
    public interface IMuseClient
    {
        event EventHandler<MuseClientNotifyTelemetryEventArgs> NotifyTelemetry;
        event EventHandler<MuseClientNotifyAccelerometerEventArgs> NotifyAccelerometer;
        event EventHandler<MuseClientNotifyGyroscopeEventArgs> NotifyGyroscope;
        event EventHandler<MuseClientNotifyEegEventArgs> NotifyEeg;

        bool Connected { get; }

        Task<bool> Connect(ulong deviceAddress);
        Task Disconnect();
        Task Subscribe(params Channel[] channels);
        Task Resume();
        Task Start();
        Task Pause();
        Task UnsubscribeAll();
        Task<bool> SubscribeToChannel(Channel channel);
        Task<bool> UnsubscribeFromChannel(Channel channel);
        Task<byte[]> SingleChannelEventAsync(Channel channel);
        Task<Telemetry> ReadTelemetryAsync();
    }
}
