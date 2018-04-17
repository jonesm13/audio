namespace Domain.Features.Audio
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using DataModel;
    using DataModel.Entities;
    using FluentValidation;
    using Helpers;
    using MediatR;
    using Pipeline;
    using Ports;

    public class Delete
    {
        public class Command : IRequest<CommandResult>
        {
            public Guid AudioId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            readonly AudioDbContext db;

            public Validator(AudioDbContext db)
            {
                this.db = db;

                RuleFor(x => x.AudioId)
                    .Must(Exist)
                    .WithHttpStatusCode(HttpStatusCode.NotFound);
            }

            bool Exist(Guid arg)
            {
                return db.Audio.Any(x => x.Id == arg);
            }
        }

        public class Handler : EntityFrameworkCommandHandler<Command, CommandResult>
        {
            readonly IAudioStore audioStore;

            public Handler(AudioDbContext db, IAudioStore audioStore) : base(db)
            {
                this.audioStore = audioStore;
            }

            protected override async Task<CommandResult> HandleImpl(Command request)
            {
                await audioStore.DeleteAsync(request.AudioId);

                AudioItem item = await Db.Audio
                    .SingleAsync(x => x.Id == request.AudioId);

                Db.Entry(item).State = EntityState.Deleted;

                return CommandResult.Void;
            }
        }
    }
}