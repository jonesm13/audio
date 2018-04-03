namespace Domain.Features.Audio
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Threading.Tasks;
    using DataModel;
    using DataModel.Entities;
    using FluentValidation;
    using Helpers;
    using MediatR;
    using Pipeline;
    using Ports;

    public class Load
    {
        public class Command : IRequest<CommandResult>
        {
            public string FileName { get; set; }
            public string Title { get; set; }
            public string[] Categories { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Title)
                    .NotNull()
                    .NotEmpty();

                RuleFor(x => x.FileName)
                    .Must(Exist);
            }

            bool Exist(string arg)
            {
                string fullFilePath = Path.Combine(
                    ConfigurationManager.AppSettings["audio:inboxPath"],
                    arg);

                return File.Exists(fullFilePath);
            }
        }

        public class Handler : EntityFrameworkCommandHandler<Command, CommandResult>
        {
            readonly IAudioStore audioStore;

            public Handler(AudioDbContext db, IAudioStore audioStore) : base(db)
            {
                this.audioStore = audioStore;
            }

            protected override Task<CommandResult> HandleImpl(Command request)
            {
                Guid id = SequentualGuid.New();

                audioStore.StoreAsync(id, request.FileName);

                Db.Audio.Add(
                    new AudioItem
                    {
                        Id = id,
                        Flags = AudioItemFlags.None,
                        Title = request.Title
                    });

                CommandResult result = CommandResult.Void
                    .WithNotification(
                        new AudioLoaded
                        {
                            Id = id,
                            Title = request.Title
                        });

                return Task.FromResult(result);
            }
        }

        public class AudioLoaded : INotification
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
        }
    }
}