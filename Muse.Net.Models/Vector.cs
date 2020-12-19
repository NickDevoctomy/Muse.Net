namespace Muse.Net.Models
{
    public struct Vector
    {
        public float X;
        public float Y;
        public float Z;

        // !!!
        public static Vector operator *(Vector vector, float f)
        {
            return new Vector { X = vector.X * f, Y = vector.Y * f, Z = vector.Z * f };
        }
    }
}
