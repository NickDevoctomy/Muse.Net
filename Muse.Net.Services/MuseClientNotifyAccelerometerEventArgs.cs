using Muse.Net.Models;
using System;

namespace Muse.Net.Services
{
    public class MuseClientNotifyAccelerometerEventArgs : EventArgs
    {
        public Accelerometer Accelerometer { get; set; }
    }
}
