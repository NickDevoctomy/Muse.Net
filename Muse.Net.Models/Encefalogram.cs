using System;

namespace Muse.Net.Models
{
    public class Encefalogram
    {
        public float Index;                     //?
        public DateTimeOffset Timestamp;
        public float[] Samples;                 // Each message holds 12 samples each time
        public byte[] Raw;
    }
}