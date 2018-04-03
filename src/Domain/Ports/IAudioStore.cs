namespace Domain.Ports
{
    using System;
    using System.Threading.Tasks;

    public interface IAudioStore
    {
        Task StoreAsync(Guid id, string filename);
        Task DeleteAsync(Guid id);
    }
}