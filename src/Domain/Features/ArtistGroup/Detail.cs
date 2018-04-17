namespace Domain.Features.ArtistGroup
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataModel;
    using DataModel.Entities;
    using MediatR;

    public class Detail
    {
        public class Query : IRequest<Model>
        {
            public string Name { get; set; }
        }

        public class Handler : AsyncRequestHandler<Query, Model>
        {
            readonly AudioDbContext db;

            public Handler(AudioDbContext db)
            {
                this.db = db;
            }

            protected override async Task<Model> HandleCore(Query request)
            {
                ArtistGroup g = await db.ArtistGroups
                    .Include(x => x.Members)
                    .SingleAsync(x => x.Name == request.Name);

                return new Model
                {
                    Name = g.Name,
                    Artists = g.Members
                        .Select(x => x.Name)
                        .ToArray()
                };
            }
        }

        public class Model
        {
            public string Name { get; set; }
            public string[] Artists { get; set; }
        }
    }
}