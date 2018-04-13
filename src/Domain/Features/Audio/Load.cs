namespace Domain.Features.Audio
{
    using System;
    using System.IO;
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
            public Validator()
            {
                CascadeMode = CascadeMode.StopOnFirstFailure;

                RuleFor(x => x.FileName)
                    .Must(Exist)
                    .Must(HaveWavExtension)
                    .Must(BeAcceptableFormat);
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

            protected override Task<CommandResult> HandleImpl(Command request)
            {
                Guid id = SequentualGuid.New();

                string fullFilePath = Path.Combine(
                    Settings.Settings.Inbox.InboxFolder,
                    request.FileName);

                audioStore.StoreAsync(id, fullFilePath);

                string title = string.IsNullOrWhiteSpace(request.Title) ?
                    request.FileName :
                    request.Title;

                Db.Audio.Add(
                    new AudioItem
                    {
                        Id = id,
                        Flags = AudioItemFlags.None,
                        Title = title,
                        Duration = (int)request.Details.Duration.TotalMilliseconds
                    });

                CommandResult result = CommandResult.Void
                    .WithNotification(
                        new AudioLoaded
                        {
                            Id = id,
                            Title = request.Title
                        });

                File.Delete(fullFilePath);

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