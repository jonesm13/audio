namespace Domain.Features.Audio.Categories
{
    using System;
    using System.Threading.Tasks;
    using DataModel;
    using MediatR;
    using Pipeline;

    public class Remove
    {
        public class Command : IRequest<CommandResult>
        {
            public Guid Id { get; set; }
            public string Category { get; set; }
        }

        public class Handler : EntityFrameworkCommandHandler<Command, CommandResult>
        {
            public Handler(AudioDbContext db) : base(db)
            {
            }

            protected override Task<CommandResult> HandleImpl(Command request)
            {
                throw new NotImplementedException();
            }
        }
    }
}