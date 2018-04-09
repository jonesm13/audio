namespace Domain.Features.Category
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataModel;
    using DataModel.Entities;
    using MediatR;

    public class Index
    {
        public class Query : IRequest<IEnumerable<Model>>
        {
        }

        public class Handler : AsyncRequestHandler<Query, IEnumerable<Model>>
        {
            readonly AudioDbContext db;

            public Handler(AudioDbContext db)
            {
                this.db = db;
            }

            protected override async Task<IEnumerable<Model>> HandleCore(Query request)
            {
                List<Model> result = new List<Model>();

                List<Category> categories = await db
                    .Categories
                    .AsNoTracking()
                    .OrderBy(x=>x.ParentId)
                    .ThenBy(x=>x.Name)
                    .ToListAsync();

                return result;
            }
        }

        public class Model
        {
            public string Path { get; set; }
            public List<Model> Children { get; set; }
        }
    }
}