using Muse.Net.Models;
using System;

namespace Muse.Net.Services
{
    public class MuseClientNotifyTelemetryEventArgs : EventArgs
    {
        public Telemetry Telemetry { get; set; }
    }
}
