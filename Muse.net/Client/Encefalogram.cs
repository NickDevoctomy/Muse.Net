using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muse.Net.Client
{
    public class Encefalogram
    {
        public float Index;                     //?
        public DateTimeOffset Timestamp;
        public float[] Samples;                 // Each message holds 12 samples each time
        public byte[] Raw;
    }
}
