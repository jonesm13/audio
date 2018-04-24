namespace Domain.Features.Audio.Markers
{
    using System;
    using MediatR;
    using Pipeline;

    public class Delete
    {
        public class Command : IRequest<CommandResult>
        {
            public Guid Id { get; set; }
            public long Offset { get; set; }
            public string Type { get; set; }
        }
    }
}