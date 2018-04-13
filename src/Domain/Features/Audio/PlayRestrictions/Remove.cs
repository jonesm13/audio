namespace Domain.Features.Audio.PlayRestrictions
{
    using System;
    using MediatR;
    using Pipeline;

    public class Remove
    {
        public class Command : IRequest<CommandResult>
        {
            public Guid Id { get; set; }
            public Guid RestrictionId { get; set; }
        }
    }
}