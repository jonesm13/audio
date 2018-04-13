namespace AudioServices.NAudio
{
    using System;
    using Domain.Ports;
    using global::NAudio.Wave;

    public class NAudioValidator : IExamineAudioFiles
    {
        public AudioFileDetails GetAudioFileDetails(string filename)
        {
            AudioFormat format = AudioFormat.Unknown;
            TimeSpan duration;

            try
            {
                using (WaveFileReader reader = new WaveFileReader(filename))
                {
                    if (reader.WaveFormat.SampleRate == 44100 &&
                        reader.WaveFormat.BitsPerSample == 16 &&
                        reader.WaveFormat.Encoding == WaveFormatEncoding.Pcm &&
                        reader.WaveFormat.Channels == 2 &&
                        reader.WaveFormat.BlockAlign == 4)
                    {
                        format = AudioFormat.RedBook;
                    }

                    duration = reader.TotalTime;
                }
            }
            catch (FormatException)
            {
                return new AudioFileDetails(AudioFormat.Unknown, TimeSpan.Zero);
            }

            return new AudioFileDetails(format, duration);
        }
    }
}
