namespace Api.Notifications
{
    using System.Threading.Tasks;
    using Domain.Features.Audio;
    using MediatR;

    public class AudioLoadedHandler : AsyncNotificationHandler<Load.AudioLoaded>
    {
        protected override Task HandleCore(Load.AudioLoaded notification)
        {
            return Task.CompletedTask;
        }
    }
}