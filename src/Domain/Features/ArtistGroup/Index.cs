namespace Domain.Features.ArtistGroup
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aspects.Pagination;
    using DataModel;
    using MediatR;

    public class Index
    {
        public class Query : PagedRequest<Model>
        {
        }

        public class Handler : AsyncRequestHandler<Query, Page<Model>>
        {
            readonly AudioDbContext db;

            public Handler(AudioDbContext db)
            {
                this.db = db;
            }

            protected override async Task<Page<Model>> HandleCore(Query request)
            {
                return await db.ArtistGroups
                    .AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Select(x => new Model {Name = x.Name})
                    .InPagesOf(request.PageSize)
                    .ToPageAsync(request.PageNumber);
            }
        }

        public class Model
        {
            public string Name { get; set; }
        }
    }
}