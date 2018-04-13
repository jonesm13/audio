namespace Domain.Features.Category
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataModel;
    using DataModel.Entities;
    using MediatR;
    using Newtonsoft.Json;

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
                    .OrderBy(x => x.ParentId)
                    .ThenBy(x => x.Name)
                    .ToListAsync();

                foreach (Category c in categories.Where(x => x.ParentId == default(Guid?)))
                {
                    Model rootNode = new Model
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Children = new List<Model>()
                    };

                    result.Add(rootNode);

                    AppendChildren(rootNode, categories);
                }

                return result;
            }

            static void AppendChildren(Model node, List<Category> categories)
            {
                IEnumerable<Model> children = categories
                    .Where(x => x.ParentId == node.Id)
                    .Select(x => new Model { Id = x.Id, Name = x.Name });

                node.Children = children.ToList();

                foreach (Model child in node.Children)
                {
                    AppendChildren(child, categories);
                }
            }
        }

        public class Model
        {
            [JsonIgnore]
            public Guid Id { get; set; }
            public string Name { get; set; }
            public List<Model> Children { get; set; }
        }
    }
}