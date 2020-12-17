using Harthoorn.MuseClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Muse.Net.Services
{
    public class MuseSamplerService : IMuseSamplerService
    {
        private DateTime? _firstSampledAt;
        private readonly MuseSamplerServiceConfiguration _configuration;
        private readonly Dictionary<Channel, List<RawSamplePacket>> _samples;
        private readonly object _lock = new object();

        public MuseSamplerService(MuseSamplerServiceConfiguration configuration)
        {
            _configuration = configuration;
            _samples = new Dictionary<Channel, List<RawSamplePacket>>();
        }

        public void Sample(
            Channel channel,
            float[] values)
        {
            lock(_lock)
            {
                var existingValues = default(List<RawSamplePacket>);
                if (!_samples.TryGetValue(
                    channel,
                    out existingValues))
                {
                    existingValues = new List<RawSamplePacket>();
                    _samples.Add(
                        channel,
                        existingValues);
                }

                existingValues.Add(new RawSamplePacket
                {
                    Channel = channel,
                    Values = values
                });

                if (_firstSampledAt != null && (DateTime.UtcNow - _configuration.SamplePeriod) > _firstSampledAt)
                {
                    TrimSamplePeriod(_configuration.SamplePeriod);
                }

                if(_firstSampledAt == null)
                {
                    _firstSampledAt = DateTime.UtcNow;
                }
            }
        }

        public bool TryGetSamples(
            Channel channel,
            TimeSpan timeSpan,
            out float[] samples)
        {
            samples = null;
            lock (_lock)
            {
                var cutOffPoint = DateTime.UtcNow - timeSpan;
                var rawSamples = default(List<RawSamplePacket>);
                if (_samples.TryGetValue(
                    channel,
                    out rawSamples))
                {
                    if (_samples.Any())
                    {
                        samples = rawSamples.Where(x => x.DateTime > cutOffPoint).SelectMany(y => y.Values).ToArray();
                        return true;
                    }
                }
            }

            return false;
        }

        private void TrimSamplePeriod(TimeSpan timeSpan)
        {
            var cutOffPoint = DateTime.UtcNow - timeSpan;
            foreach (Channel curChannel in _samples.Keys)
            {
                var rawSamples = default(List<RawSamplePacket>);
                if (_samples.TryGetValue(
                    curChannel,
                    out rawSamples))
                {
                    if(_samples.Any())
                    {
                        var last = rawSamples.LastOrDefault(x => x.DateTime < cutOffPoint);
                        if (last != null)
                        {
                            var lastIndex = rawSamples.IndexOf(last);
                            rawSamples.RemoveRange(0, lastIndex + 1);
                        }
                    }
                }
            }
        }
    }
}
