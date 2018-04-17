namespace Domain.Features.Artist
{
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
        }

        public class Validator : AbstractValidator<Command>
        {
            readonly AudioDbContext db;

            public Validator(AudioDbContext db)
            {
                this.db = db;

                RuleFor(x => x.Name)
                    .Must(NotExist)
                    .WithHttpStatusCode(HttpStatusCode.Conflict);
            }

            bool NotExist(string arg)
            {
                return !db.Artists.Any(x => x.Name.Equals(arg));
            }
        }

        public class Handler : EntityFrameworkCommandHandler<Command, CommandResult>
        {
            public Handler(AudioDbContext db) : base(db)
            {
            }

            protected override Task<CommandResult> HandleImpl(Command request)
            {
                Db.Artists.Add(new Artist
                {
                    Id = SequentualGuid.New(),
                    Name = request.Name
                });

                return Task.FromResult(CommandResult.Void);
            }
        }
    }
}
