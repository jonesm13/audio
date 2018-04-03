namespace Domain.Features.Audio.Categories
{
    using System;
    using MediatR;
    using Pipeline;

    public class Add
    {
        public class Command : IRequest<CommandResult>
        {
            public Guid AudioId { get; set; }
            public string Category { get; set; }
        }
    }
}