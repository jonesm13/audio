namespace Domain.Features.Artist
{
    using System.Linq;
    using System.Threading.Tasks;
    using Aspects.Pagination;
    using DataModel;
    using DataModel.Entities;
    using MediatR;

    public class Index
    {
        public class Query : PagedRequest<Model>
        {
            public string Search { get; set; }
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
                IQueryable<Artist> query = db.Artists.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    query = query.Where(x => x.Name.Contains(request.Search));
                }

                return await query
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