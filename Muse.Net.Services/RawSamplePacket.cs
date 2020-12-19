using Muse.Net.Models.Enums;
using System;

namespace Muse.Net.Services
{
    public class RawSamplePacket
    {
        public DateTime DateTime { get; } = DateTime.UtcNow;
        public Channel Channel { get; set; }
        public float[] Values { get; set; }
    }
}