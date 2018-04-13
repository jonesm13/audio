namespace Domain.Features.Audio.Artists
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataModel;
    using DataModel.Entities;
    using MediatR;
    using Pipeline;

    public class Add
    {
        public class Command : IRequest<CommandResult>
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class Handler : EntityFrameworkCommandHandler<Command, CommandResult>
        {
            public Handler(AudioDbContext db) : base(db)
            {
            }

            protected override async Task<CommandResult> HandleImpl(Command request)
            {
                AudioItem item = await Db.Audio
                    .SingleAsync(x => x.Id == request.Id);

                Artist artist = await Db.Artists
                    .SingleAsync(x => x.Name == request.Name);

                item.Artists.Add(artist);

                return CommandResult.Void;
            }
        }
    }
}