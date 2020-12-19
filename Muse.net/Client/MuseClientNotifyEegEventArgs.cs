using Muse.Net.Models;
using Muse.Net.Models.Enums;
using System;

namespace Muse.Net.Client
{
    public class MuseClientNotifyEegEventArgs : EventArgs
    {
        public Channel Channel { get; set; }
        public Encefalogram Encefalogram { get; set; }
    }
}
