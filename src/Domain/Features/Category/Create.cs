namespace Domain.Features.Category
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;
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
            public string Path { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            readonly AudioDbContext db;

            public Validator(AudioDbContext db)
            {
                this.db = db;

                RuleFor(x => x.Path)
                    .Must(NotBeNullOrEmpty)
                    .MustAsync(BeValid);
            }

            bool NotBeNullOrEmpty(string arg)
            {
                return !string.IsNullOrWhiteSpace(arg);
            }

            async Task<bool> BeValid(string arg, CancellationToken cancellationToken)
            {
                List<Category> categories = await db.Categories
                    .AsNoTracking()
                    .OrderBy(x => x.ParentId)
                    .ThenBy(x => x.Name)
                    .ToListAsync(cancellationToken);

                string[] split = arg.Split(
                    new[] {'/'},
                    StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 1)
                {
                    return !categories.Any(x =>
                        x.Name == split[0] &&
                        x.ParentId == default(Guid?));
                }

                string parentPath = string.Join(
                    "/",
                    split.Take(split.Length - 1));

                Category parentNode = categories.FindNode(
                    parentPath,
                    c => c.Id,
                    c => c.ParentId,
                    c => c.Name);

                return parentNode != null;
            }
        }

        public class Handler : EntityFrameworkCommandHandler<Command, CommandResult>
        {
            public Handler(AudioDbContext db) : base(db)
            {
            }

            protected override async Task<CommandResult> HandleImpl(Command request)
            {
                List<Category> categories = await Db.Categories
                    .AsNoTracking()
                    .OrderBy(x => x.ParentId)
                    .ThenBy(x => x.Name)
                    .ToListAsync();

                string[] pathSplit = request
                    .Path
                    .Split(new [] { '/' },
                        StringSplitOptions.RemoveEmptyEntries);

                string parentPath = string.Join(
                    "/",
                    pathSplit.Take(pathSplit.Length - 1));

                Category parent = categories.FindNode(
                    parentPath,
                    x => x.Id,
                    x => x.ParentId,
                    x => x.Name);

                Db.Categories.Add(new Category
                {
                    Id = SequentualGuid.New(),
                    ParentId = parent?.Id,
                    Name = pathSplit.Last(),
                });

                return CommandResult.Void;
            }
        }
    }
}
