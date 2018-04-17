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

    public class Create
    {
        public class Command : IRequest<CommandResult>
        {
            public string Name { get; set; }
            public string[] Members { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            readonly AudioDbContext db;

            public Validator(AudioDbContext db)
            {
                this.db = db;

                RuleFor(x => x.Name)
                    .Must(NotAlreadyExist)
                    .WithHttpStatusCode(HttpStatusCode.Conflict);

                RuleForEach(x => x.Members)
                    .Must(Exist)
                    .WithHttpStatusCode(HttpStatusCode.NotFound);
            }

            bool NotAlreadyExist(string arg)
            {
                return db.ArtistGroups.Any(x => x.Name.Equals(arg));
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
                ArtistGroup newGroup = new ArtistGroup
                {
                    Id = SequentualGuid.New(),
                    Name = request.Name
                };

                if (request.Members.Any())
                {
                    foreach (string artistName in request.Members)
                    {
                        Artist theArtist = await Db
                            .Artists
                            .SingleAsync(x => x.Name.Equals(artistName));

                        newGroup.Members.Add(theArtist);
                    }
                }

                Db.ArtistGroups.Add(newGroup);

                return CommandResult.Void;
            }
        }
    }
}