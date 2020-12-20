using Muse.Net.Models;
using System;

namespace Muse.Net.Services
{
    public interface IMuseDataParserService
    {
        Telemetry Telemetry(byte[] span);
        Gyroscope Gyroscope(byte[] span);
        Accelerometer Accelerometer(byte[] span);
        Encefalogram Encefalogram(byte[] span);
        float[] EegSamples(ReadOnlySpan<byte> span);
        void ScaleEeg(float[] samples);
        Vector[] Samples(ReadOnlySpan<byte> span, int count, float scale);
        Vector Vector(ReadOnlySpan<byte> span);
        ushort UShort(ReadOnlySpan<byte> span, int index = 0);
        short Int16(ReadOnlySpan<byte> span, int index = 0);
    }
}
