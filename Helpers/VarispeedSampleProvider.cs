using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Spotify.Helpers
{

    public class VarispeedSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider sourceProvider;
        private float playbackRate;
        private float[] sourceBuffer;
        private int sourceBufferIndex;
        private int sourceBufferCount;
        private double currentTime;
        private int totalSamplesRead;
        private int totalSamplesProcessed;

        public VarispeedSampleProvider(ISampleProvider sourceProvider)
        {
            this.sourceProvider = sourceProvider;
            this.playbackRate = 1.0f;
            // Smaller buffer size to prevent large chunks
            this.sourceBuffer = new float[sourceProvider.WaveFormat.SampleRate / 4];
            this.sourceBufferIndex = 0;
            this.sourceBufferCount = 0;
            this.totalSamplesRead = 0;
            this.totalSamplesProcessed = 0;
            this.currentTime = 0;
        }

        public float PlaybackRate
        {
            get { return playbackRate; }
            set
            {
                if (value <= 0) throw new ArgumentException("Playback rate must be greater than zero");
                playbackRate = value;
                // Reset buffers when speed changes to prevent artifacts
                sourceBufferIndex = 0;
                sourceBufferCount = 0;
                currentTime = 0;
            }
        }

        public WaveFormat WaveFormat => sourceProvider.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int outputIndex = offset;
            int outputEndIndex = offset + count;

            while (outputIndex < outputEndIndex)
            {
                try
                {
                    // Check if we need to refill the source buffer
                    if (sourceBufferIndex >= sourceBufferCount)
                    {
                        if (sourceProvider == null)
                        {
                            throw new ObjectDisposedException(nameof(sourceProvider), "The source provider has been disposed.");
                        }

                        sourceBufferCount = sourceProvider.Read(sourceBuffer, 0, sourceBuffer.Length);
                        sourceBufferIndex = 0;

                        if (sourceBufferCount == 0)
                        {
                            break; // End of source audio
                        }
                        totalSamplesRead += sourceBufferCount;
                    }

                    // Determine how to process based on playback rate
                    if (Math.Abs(playbackRate - 1.0f) < 0.001f)
                    {
                        // Normal speed (1x)
                        while (outputIndex < outputEndIndex && sourceBufferIndex < sourceBufferCount)
                        {
                            buffer[outputIndex++] = sourceBuffer[sourceBufferIndex++];
                            totalSamplesProcessed++;
                        }
                    }
                    else if (playbackRate >= 1.0f)
                    {
                        // Speeds greater than 1x (faster playback)
                        while (outputIndex < outputEndIndex && sourceBufferIndex < sourceBufferCount)
                        {
                            int sourceSample = (int)currentTime;

                            if (sourceSample >= sourceBufferCount)
                            {
                                break;
                            }

                            buffer[outputIndex++] = sourceBuffer[sourceSample];
                            currentTime += playbackRate;
                            totalSamplesProcessed++;
                        }
                    }
                    else
                    {
                        // Speeds less than 1x (slower playback)
                        while (outputIndex < outputEndIndex)
                        {
                            // Ensure we have enough source samples
                            if (sourceBufferIndex >= sourceBufferCount)
                            {
                                sourceBufferCount = sourceProvider.Read(sourceBuffer, 0, sourceBuffer.Length);
                                sourceBufferIndex = 0;

                                if (sourceBufferCount == 0)
                                    break;
                            }

                            // Calculate how many source samples to skip
                            int samplesToSkip = (int)(1.0f / playbackRate);

                            // Linear interpolation for slower playback
                            if (sourceBufferIndex + 1 < sourceBufferCount)
                            {
                                float sample1 = sourceBuffer[sourceBufferIndex];
                                float sample2 = sourceBuffer[sourceBufferIndex + 1];

                                buffer[outputIndex++] = sample1;

                                // Advance source buffer
                                sourceBufferIndex += samplesToSkip;
                                totalSamplesProcessed++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    // Reset current time if we've processed all samples
                    if (currentTime >= sourceBufferCount && sourceBufferCount > 0)
                    {
                        currentTime = 0;
                        sourceBufferIndex = sourceBufferCount;
                    }
                }
                catch (System.Runtime.InteropServices.InvalidComObjectException ex)
                {
                    // Handle the specific exception
                    Console.WriteLine($"Invalid COM object: {ex.Message}");
                    break;
                }
            }

            return outputIndex - offset;
        }

        // Helper method to get current position
        public TimeSpan GetCurrentTime()
        {
            return TimeSpan.FromSeconds((double)totalSamplesProcessed / WaveFormat.SampleRate);
        }
    }
}
