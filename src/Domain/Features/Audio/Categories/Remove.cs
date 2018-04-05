namespace Domain.Features.Audio.Categories
{
    using System;
    using MediatR;
    using Pipeline;

    public class Remove
    {
        public class Command : IRequest<CommandResult>
        {
            public Guid Id { get; set; }
            public string Category { get; set; }
        }
    }
}