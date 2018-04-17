namespace Domain.Features.Audio.Categories
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using DataModel;
    using DataModel.Entities;
    using FluentValidation;
    using Helpers;
    using MediatR;
    using Pipeline;

    public class Remove
    {
        public class Command : IRequest<CommandResult>
        {
            public Guid Id { get; set; }
            public string Category { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            readonly AudioDbContext db;

            public Validator(AudioDbContext db)
            {
                this.db = db;

                RuleFor(x => x.Id)
                    .Must(AudioItemExist)
                    .WithHttpStatusCode(HttpStatusCode.NotFound);

                RuleFor(x => x.Category)
                    .Must(CategoryExist)
                    .WithHttpStatusCode(HttpStatusCode.NotFound);
            }

            bool CategoryExist(string arg)
            {
                Category category = db.Categories
                    .AsNoTracking()
                    .ToList()
                    .FindNode(arg, x => x.Id, x => x.ParentId, x => x.Name);

                return category != null;
            }

            bool AudioItemExist(Guid arg)
            {
                return db.Audio.Any(x => x.Id == arg);
            }
        }

        public class Handler : EntityFrameworkCommandHandler<Command, CommandResult>
        {
            public Handler(AudioDbContext db) : base(db)
            {
            }

            protected override async Task<CommandResult> HandleImpl(Command request)
            {
                AudioItem item = await Db.Audio
                    .Include(x => x.Categories)
                    .SingleAsync(x => x.Id == request.Id);

                Category category = Db.Categories
                    .AsNoTracking()
                    .ToList()
                    .FindNode(request.Category, x => x.Id, x => x.ParentId, x => x.Name);

                Category cat = item.Categories
                    .Single(x => x.Id == category.Id);

                item.Categories.Remove(cat);

                return CommandResult.Void;
            }
        }
    }
}