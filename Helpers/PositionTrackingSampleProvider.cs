using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Helpers
{
    public class PositionTrackingSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;
        private long totalSamples;

        public PositionTrackingSampleProvider(ISampleProvider source)
        {
            this.source = source;
            this.totalSamples = 0;
        }

        public WaveFormat WaveFormat => source.WaveFormat;

        public TimeSpan CurrentTime =>
            TimeSpan.FromSeconds((double)totalSamples / WaveFormat.SampleRate);

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = source.Read(buffer, offset, count);
            totalSamples += samplesRead;
            return samplesRead;
        }
    }
}
