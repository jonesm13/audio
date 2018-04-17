namespace Domain.Features.ArtistGroup
{
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
            public string Name { get; set; }
            public string ArtistName { get; set; }
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

                RuleFor(x => x.ArtistName)
                    .Must(ArtistExist)
                    .WithHttpStatusCode(HttpStatusCode.NotFound);
            }

            bool ArtistExist(string arg)
            {
                return db.Artists.Any(x => x.Name.Equals(arg));
            }

            bool Exist(string arg)
            {
                return db.ArtistGroups.Any(x => x.Name.Equals(arg));
            }
        }

        public class Handler : EntityFrameworkCommandHandler<Command, CommandResult>
        {
            public Handler(AudioDbContext db) : base(db)
            {
            }

            protected override async Task<CommandResult> HandleImpl(Command request)
            {
                ArtistGroup g = await Db.ArtistGroups
                    .Include(x => x.Members)
                    .SingleAsync(x => x.Name == request.Name);

                Artist a = await Db.Artists
                    .SingleAsync(x => x.Name == request.ArtistName);

                g.Members.Add(a);

                return CommandResult.Void;
            }
        }
    }
}
