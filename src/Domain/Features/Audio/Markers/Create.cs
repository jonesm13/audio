namespace Domain.Features.Audio.Markers
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using DataModel;
    using DataModel.Entities;
    using Helpers;
    using MediatR;
    using Pipeline;

    public class Create
    {
        public class Command : IRequest<CommandResult>
        {
            public Guid Id { get; set; }
            public long Offset { get; set; }
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

                item.Markers.Add(new Marker
                {
                    Id = SequentualGuid.New(),
                    Offset = request.Offset
                });

                return CommandResult.Void;
            }
        }
    }
}