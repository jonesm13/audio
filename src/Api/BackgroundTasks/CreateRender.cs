namespace Api.BackgroundTasks
{
    using System;
    using System.Threading.Tasks;

    public class CreateRender
    {
        public Task Go(Guid audioId)
        {
            return Task.CompletedTask;
        }
    }
}
