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

    public static class MarkerTypes
    {
        public static string[] KnownTypes = new[]
        {
            "introStart",
            "introEnd",
            "hookStart",
            "hookEnd",
            "segue",
            "command"
        };

        public static string BuildCustomTypeName(string name)
        {
            return $"{CustomPrefix}{name}";
        }

        public static string CustomPrefix = "Custom:";
    }

    public class Create
    {
        public class Command : IRequest<CommandResult>
        {
            public Guid Id { get; set; }
            public long Offset { get; set; }
            public string Type { get; set; }
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

                RuleFor(x => x)
                    .MustAsync(NotExistAlready);
            }

            async Task<bool> NotExistAlready(
                Command arg,
                CancellationToken cancellationToken)
            {
                AudioItem item = await db.Audio
                    .AsNoTracking()
                    .Include(x => x.Markers)
                    .SingleAsync(x => x.Id == arg.Id, cancellationToken);

                return !item.Markers
                    .Any(x => x.Offset == arg.Offset && x.Type == arg.Type);
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

                string type = MarkerTypes.KnownTypes.Contains(request.Type) ?
                    request.Type :
                    MarkerTypes.BuildCustomTypeName(request.Type);

                item.Markers.Add(new Marker
                {
                    Id = SequentualGuid.New(),
                    Offset = request.Offset,
                    Type = type
                });

                return CommandResult.Void;
            }
        }
    }
}