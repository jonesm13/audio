namespace Domain.Features.Audio
{
    using System;
    using MediatR;
    using Pipeline;

    public class Replace
    {
        public class Command : IRequest<CommandResult>
        {
            public Guid Id { get; set; }
            public string FileName { get; set; }
        }
    }
}