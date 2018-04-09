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
    using Pipeline;

    public class Delete
    {
        public class Command : IRequest<CommandResult>
        {
            public string Path { get; set; }
        }

        public class Handler : EntityFrameworkCommandHandler<Command, CommandResult>
        {
            public Handler(AudioDbContext db) : base(db)
            {
            }

            protected override async Task<CommandResult> HandleImpl(Command request)
            {
                List<Category> categories = await Db.Categories
                    .OrderBy(x => x.ParentId)
                    .ThenBy(x => x.Name)
                    .ToListAsync();

                string[] split = request.Path.Split(
                    new[] { '/' },
                    StringSplitOptions.RemoveEmptyEntries);

                Category item = null;

                foreach (string s in split.Take(split.Length))
                {
                    item = categories
                        .First(x => x.Name == s &&
                            x.ParentId == item?.Id);
                }

                Db.Entry(item).State = EntityState.Deleted;

                return CommandResult.Void;
            }
        }
    }
}