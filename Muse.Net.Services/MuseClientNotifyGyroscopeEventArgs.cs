using Muse.Net.Models;
using System;

namespace Muse.Net.Services
{
    public class MuseClientNotifyGyroscopeEventArgs : EventArgs
    {
        public Gyroscope Gyroscope { get; set; }
    }
}
