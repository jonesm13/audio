namespace Domain.Adapters
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Ports;

    public class SqlFileStreamAudioStore : IAudioStore
    {
        public Task StoreAsync(Guid id, string filename)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            return Task.CompletedTask;
        }

        public Task<Stream> Get(Guid id)
        {
            Stream result = new MemoryStream();
            return Task.FromResult(result);
        }
    }
}
