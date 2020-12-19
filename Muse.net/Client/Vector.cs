using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muse.Net.Client
{
    public struct Vector
    {
        public float X;
        public float Y;
        public float Z;

        public static Vector operator *(Vector vector, float f)
        {
            return new Vector { X = vector.X * f, Y = vector.Y * f, Z = vector.Z * f };
        }
    }
}
