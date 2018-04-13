namespace Domain.Features.Category
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataModel;
    using DataModel.Entities;
    using FluentValidation;
    using Helpers;
    using MediatR;
    using Pipeline;

    public class Delete
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
                    .Must(Exist)
                    .Must(ContainNoAudio)
                    .Must(ContainNoChildren);
            }

            bool ContainNoChildren(string arg)
            {
                Category cat = db.Categories
                    .AsNoTracking()
                    .FindNode(arg);

                return !db.Categories.Any(x => x.ParentId == cat.Id);
            }

            bool ContainNoAudio(string arg)
            {
                Category cat = db.Categories
                    .AsNoTracking()
                    .FindNode(arg);

                return !db.Audio.Any(x => x.Categories.Any(y => y.Id == cat.Id));
            }

            bool Exist(string arg)
            {
                Category cat = db.Categories
                    .AsNoTracking()
                    .FindNode(arg);

                return cat != null;
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
                    .OrderBy(x => x.ParentId)
                    .ThenBy(x => x.Name)
                    .ToListAsync();

                Category item = categories.FindNode(request.Path);

                Db.Entry(item).State = EntityState.Deleted;

                return CommandResult.Void;
            }
        }
    }
}