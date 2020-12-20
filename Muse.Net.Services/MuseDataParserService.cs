using Muse.Net.Models;
using System;
using System.Buffers.Binary;

namespace Muse.Net.Services
{
    public class MuseDataParserService : IMuseDataParserService
    {
        public Telemetry Telemetry(byte[] span)
        {
            return new Telemetry
            {
                SequenceId = UShort(span, 0),
                BatteryLevel = UShort(span, 2) / 512f,      // percentage
                Voltage = UShort(span, 4) * 2.2f,           // don't know why
                Temperature = UShort(span, 8)
            };
        }

        public Gyroscope Gyroscope(byte[] span)
        {
            var roSpan = new ReadOnlySpan<byte>(span);
            return new Gyroscope
            {
                SequenceId = UShort(span, 0),
                Samples = Samples(roSpan.Slice(2), 3, Scale.GYROSCOPE)
            };
        }

        public Accelerometer Accelerometer(byte[] span)
        {
            var roSpan = new ReadOnlySpan<byte>(span);
            return new Accelerometer
            {
                SequenceId = UShort(span, 0),
                Samples = Samples(roSpan.Slice(2), 3, Scale.ACCELEROMETER)
            };
        }

        public Encefalogram Encefalogram(byte[] span)
        {
            var roSpan = new ReadOnlySpan<byte>(span);
            var samples = EegSamples(roSpan.Slice(2));
            return new Encefalogram
            {
                Index = UShort(span, 0),
                Timestamp = DateTimeOffset.Now,
                Samples = samples,
                Raw = roSpan.ToArray()
            };
        }

        public float[] EegSamples(ReadOnlySpan<byte> span)
        {
            var len = span.Length * 2 / 3;
            float[] samples = new float[len];
            int j = 0;

            for (int i = 0; i < len; i++)
            {
                int n;
                if (i % 2 == 0)
                {
                    n = (span[j] << 4) | (span[j + 1] >> 4);
                    j += 2;
                }
                else
                {
                    n = ((span[j - 1] & 0xF) << 8) | span[j];
                    j++;
                }
                samples[i] = n;
            }
            return samples;
        }

        public void ScaleEeg(float[] samples)
        {
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = Scale.EEG * (samples[i] - Scale.EEG_OFSET);
            }
        }

        public Vector[] Samples(ReadOnlySpan<byte> span, int count, float scale)
        {
            var samples = new Vector[3];
            for (int i = 0; i < count; i++)
                samples[i] = Vector(span.Slice(i * 6, 6)) * scale;

            return samples;
        }

        public Vector Vector(ReadOnlySpan<byte> span)
        {
            return new Vector
            {
                X = Int16(span, 0),
                Y = Int16(span, 2),
                Z = Int16(span, 4),
            };
        }

        public ushort UShort(ReadOnlySpan<byte> span, int index = 0)
        {
            return BinaryPrimitives.ReadUInt16BigEndian(span.Slice(index, 2));
        }

        public short Int16(ReadOnlySpan<byte> span, int index = 0)
        {
            return BinaryPrimitives.ReadInt16BigEndian(span.Slice(index, 2));
        }
    }
}
