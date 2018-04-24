namespace Domain.Features.Audio.Markers
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DataModel;
    using DataModel.Entities;
    using FluentValidation;
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

        public class Validation : AbstractValidator<Command>
        {
            readonly AudioDbContext db;

            public Validation(AudioDbContext db)
            {
                this.db = db;
                RuleFor(x => x.Id)
                    .Must(Exist);

                RuleFor(x => x.Offset)
                    .MustAsync(BeWithinAudioBounds);
            }

            bool Exist(Guid arg)
            {
                return db.Audio.Any(x => x.Id == arg);
            }

            async Task<bool> BeWithinAudioBounds(Command command, long arg, CancellationToken cancellationToken)
            {
                AudioItem item = await db.Audio
                    .AsNoTracking()
                    .SingleAsync(
                        x => x.Id == command.Id,
                        cancellationToken);

                return arg < item.Duration;
            }
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