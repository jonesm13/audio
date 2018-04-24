namespace Api.Notifications
{
    using System.Threading.Tasks;
    using BackgroundTasks;
    using Domain.Features.Audio;
    using Hangfire;
    using MediatR;

    public class AudioLoadedHandler : AsyncNotificationHandler<Load.AudioLoaded>
    {
        protected override Task HandleCore(Load.AudioLoaded notification)
        {
            BackgroundJob.Enqueue<CreateRender>(x => x.Go(notification.Id));

            return Task.CompletedTask;
        }
    }
}