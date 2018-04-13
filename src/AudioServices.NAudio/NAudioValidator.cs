namespace AudioServices.NAudio
{
    using System.IO;
    using Domain.Ports;
    using global::NAudio.Wave;

    public class NAudioValidator : IValidateAudioFiles
    {
        public AudioFormat GetAudioFormat(string filename)
        {
            using (WaveFileReader reader = new WaveFileReader(filename))
            {
                if (reader.WaveFormat.SampleRate == 44100 &&
                    reader.WaveFormat.BitsPerSample == 16 &&
                    reader.WaveFormat.Encoding == WaveFormatEncoding.Pcm &&
                    reader.WaveFormat.Channels == 2 &&
                    reader.WaveFormat.BlockAlign == 4)
                {
                    return AudioFormat.RedBook;
                }
            }

            return AudioFormat.Unknown;
        }
    }
}
