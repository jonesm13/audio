namespace Domain.Ports
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public interface IAudioStore
    {
        Task StoreAsync(Guid id, string filename);
        Task DeleteAsync(Guid id);
        Task<Stream> Get(Guid id);
    }
}