using System;

namespace Harthoorn.MuseClient
{
    public class MuseClientNotifyEegEventArgs : EventArgs
    {
        public Channel Channel { get; set; }
        public Encefalogram Encefalogram { get; set; }
    }
}
