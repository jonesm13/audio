namespace Domain.Features.Audio
{
    using System;
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
            public Guid Id { get; set; }
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
                AudioItem item = await db.Audio
                    .AsNoTracking()
                    .Include(x => x.Markers)
                    .SingleAsync(x => x.Id == request.Id);

                return new Model
                {
                    Title = item.Title,
                    Markers = item.Markers
                        .Select(x => new MarkerModel
                        {

                        })
                        .ToArray()
                };
            }
        }

        public class Model
        {
            public string Title { get; set; }
            public MarkerModel[] Markers { get; set; }
        }

        public class MarkerModel
        {
        }
    }
}