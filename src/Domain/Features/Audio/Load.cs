namespace Domain.Features.Audio
{
    using System;
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
            readonly IValidateAudioFiles audioFileValidator;

            public Validator(IValidateAudioFiles audioFileValidator)
            {
                this.audioFileValidator = audioFileValidator;

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

            bool BeAcceptableFormat(string arg)
            {
                string fullFilePath = Path.Combine(
                    Settings.Settings.Inbox.InboxFolder,
                    arg);

                AudioFormat format = audioFileValidator.GetAudioFormat(fullFilePath);

                return format.Equals(AudioFormat.RedBook);
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
                        Title = title
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