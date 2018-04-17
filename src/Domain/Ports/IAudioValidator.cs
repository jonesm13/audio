namespace Domain.Ports
{
    using System;

    public interface IExamineAudioFiles
    {
        AudioFileDetails GetAudioFileDetails(string filename);
    }

    public class AudioFileDetails
    {
        public static readonly AudioFileDetails Unknown = new AudioFileDetails(AudioFormat.Unknown, TimeSpan.Zero);

        public AudioFormat Format { get; }
        public TimeSpan Duration { get; }

        public AudioFileDetails(AudioFormat format, TimeSpan duration)
        {
            Format = format;
            Duration = duration;
        }
    }

    public class AudioFormat
    {
        public enum Encoding
        {
            Pcm
        }

        public static readonly AudioFormat RedBook = new AudioFormat(
            Encoding.Pcm,
            44100,
            16,
            2);

        public static readonly AudioFormat Unknown = new AudioFormat();

        readonly int bitDepth;
        readonly int channels;
        readonly Encoding encoding;
        readonly int samplesPerSecond;

        AudioFormat()
        {
        }

        AudioFormat(
            Encoding encoding,
            int samplesPerSecond,
            int bitDepth,
            int channels)
        {
            this.encoding = encoding;
            this.samplesPerSecond = samplesPerSecond;
            this.bitDepth = bitDepth;
            this.channels = channels;
        }

        protected bool Equals(AudioFormat other)
        {
            return encoding == other.encoding &&
                   samplesPerSecond == other.samplesPerSecond &&
                   bitDepth == other.bitDepth &&
                   channels == other.channels;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((AudioFormat) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (int) encoding;
                hashCode = (hashCode * 397) ^ samplesPerSecond;
                hashCode = (hashCode * 397) ^ bitDepth;
                hashCode = (hashCode * 397) ^ channels;
                return hashCode;
            }
        }
    }
}