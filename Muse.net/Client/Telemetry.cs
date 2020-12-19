using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muse.Net.Client
{
    public class Telemetry
    {
        public ushort SequenceId;
        public float BatteryLevel;
        public float Voltage;
        public ushort Temperature;
    }
}
