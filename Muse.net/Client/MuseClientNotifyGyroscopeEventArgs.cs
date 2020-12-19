using Muse.Net.Models;
using System;

namespace Muse.Net.Client
{
    public class MuseClientNotifyGyroscopeEventArgs : EventArgs
    {
        public Gyroscope Gyroscope { get; set; }
    }
}
