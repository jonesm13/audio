namespace Domain.Features.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DataModel;
    using DataModel.Entities;
    using FluentValidation;
    using Helpers;
    using MediatR;
    using MediatR.Pipeline;
    using Newtonsoft.Json;
    using Pipeline;
    using Ports;

    public class Load
    {
        public class Command : IRequest<CommandResult>
        {
            public string FileName { get; set; }
            public string Title { get; set; }
            public string[] Categories { get; set; }
            [JsonIgnore]
            [JsonProperty(Required = Required.Default)]
            public AudioFileDetails Details { get; set; }
        }

        public class PreRequest : IRequestPreProcessor<Command>
        {
            readonly IExamineAudioFiles audioFileValidator;

            public PreRequest(IExamineAudioFiles audioFileValidator)
            {
                this.audioFileValidator = audioFileValidator;
            }

            public Task Process(Command request, CancellationToken cancellationToken)
            {
                string fullFilePath = Path.Combine(
                    Settings.Settings.Inbox.InboxFolder,
                    request.FileName);

                request.Details = audioFileValidator.GetAudioFileDetails(
                    fullFilePath);

                return Task.CompletedTask;
            }
        }

        public class Validator : AbstractValidator<Command>
        {
            readonly AudioDbContext db;

            public Validator(AudioDbContext db)
            {
                this.db = db;
                CascadeMode = CascadeMode.StopOnFirstFailure;

                RuleFor(x => x.FileName)
                    .Must(Exist)
                    .Must(HaveWavExtension)
                    .Must(BeAcceptableFormat);

                RuleFor(x => x.Categories)
                    .MustAsync(AllExist);
            }

            async Task<bool> AllExist(
                string[] arg,
                CancellationToken cancellationToken)
            {
                if (!arg.Any())
                {
                    return true;
                }

                IEnumerable<Category> cats = await db.Categories
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return arg
                    .Select(s => cats.FindNode(s))
                    .All(node => node != null);
            }

            bool HaveWavExtension(string arg)
            {
                return arg.EndsWith(
                    ".wav",
                    StringComparison.InvariantCultureIgnoreCase);
            }

            bool BeAcceptableFormat(Command command, string filename)
            {
                return command.Details.Format.Equals(AudioFormat.RedBook);
            }

            bool Exist(string arg)
            {
                string fullFilePath = Path.Combine(
                    Settings.Settings.Inbox.InboxFolder,
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

            protected override async Task<CommandResult> HandleImpl(Command request)
            {
                Guid id = SequentualGuid.New();

                string fullFilePath = Path.Combine(
                    Settings.Settings.Inbox.InboxFolder,
                    request.FileName);

                await audioStore.StoreAsync(id, fullFilePath);

                string title = string.IsNullOrWhiteSpace(request.Title) ?
                    request.FileName :
                    request.Title;

                AudioItem newItem = new AudioItem
                {
                    Id = id,
                    Flags = AudioItemFlags.None,
                    Title = title,
                    Duration = (int)request.Details.Duration.TotalMilliseconds,
                };

                if (request.Categories.Any())
                {
                    List<Category> categories = await Db.Categories
                        .ToListAsync();

                    foreach (string path in request.Categories)
                    {
                        Category cat = categories.FindNode(path);

                        newItem.Categories.Add(cat);
                    }
                }

                Db.Audio.Add(newItem);

                CommandResult result = CommandResult.Void
                    .WithNotification(
                        new AudioLoaded
                        {
                            Id = id,
                            Title = request.Title
                        });

                File.Delete(fullFilePath);

                return result;
            }
        }

        public class AudioLoaded : INotification
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
        }
    }
}