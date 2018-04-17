namespace Domain.Features.Audio.Artists
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

    public class Add
    {
        public class Command : IRequest<CommandResult>
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            readonly AudioDbContext db;

            public Validator(AudioDbContext db)
            {
                this.db = db;

                RuleFor(x => x.Name)
                    .Must(Exist)
                    .WithHttpStatusCode(HttpStatusCode.NotFound);
            }

            bool Exist(string arg)
            {
                return db.Artists.Any(x => x.Name.Equals(arg));
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

                Artist artist = await Db.Artists
                    .SingleAsync(x => x.Name == request.Name);

                item.Artists.Add(artist);

                return CommandResult.Void;
            }
        }
    }
}