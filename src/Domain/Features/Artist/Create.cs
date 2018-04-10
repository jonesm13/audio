namespace Domain.Features.Artist
{
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
            public string Name { get; set; }
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
