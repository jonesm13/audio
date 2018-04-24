namespace Domain.Features.Audio
{
    using System;
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
                IQueryable<AudioItem> query = db.Audio.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    query = query.Where(x => x.Title.Contains(request.Search));
                }

                return await query
                    .OrderBy(x => x.Title)
                    .Select(x => new Model
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Duration = x.Duration
                    })
                    .InPagesOf(request.PageSize)
                    .ToPageAsync(request.PageNumber);
            }
        }

        public class Model
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public long Duration { get; set; }
        }
    }
}