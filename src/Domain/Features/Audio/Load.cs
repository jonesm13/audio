namespace Domain.Features.Audio
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.IO;
    using System.Threading.Tasks;
    using DataModel;
    using DataModel.Entities;
    using FluentValidation;
    using Helpers;
    using MediatR;
    using Pipeline;

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
            public Handler(AudioDbContext db) : base(db)
            {
            }

            protected override Task<CommandResult> HandleImpl(Command request)
            {
                Guid id = SequentualGuid.New();

                ImportAudioFile(id, request.FileName);

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

            void ImportAudioFile(Guid id, string filename)
            {
                string fullFilePath = Path.Combine(
                    ConfigurationManager.AppSettings["audio:inboxPath"],
                    filename);

                const string insertSql = @"INSERT INTO AudioFile (Id, [Data])
                    VALUES (@id, 0x00);

                    SELECT [Data].PathName(),
                        GET_FILESTREAM_TRANSACTION_CONTEXT()
                    FROM AudioFile
                    WHERE Id = @id;";

                string connectionString = ConfigurationManager
                    .ConnectionStrings["audio-db"]
                    .ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlTransaction txn = conn.BeginTransaction())
                    {
                        byte[] serverTxn;
                        string serverPath;

                        using (SqlCommand cmd = new SqlCommand(insertSql, conn))
                        {
                            cmd.Transaction = txn;
                            cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = id;

                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                                rdr.Read();

                                serverPath = rdr.GetSqlString(0).Value;
                                serverTxn = rdr.GetSqlBinary(1).Value;
                            }
                        }

                        const int blockSize = 1024 * 512;

                        using (FileStream source = new FileStream(
                            fullFilePath,
                            FileMode.Open,
                            FileAccess.Read))
                        {
                            using (SqlFileStream dest = new SqlFileStream(
                                serverPath,
                                serverTxn,
                                FileAccess.Write))
                            {
                                byte[] buffer = new byte[blockSize];
                                int bytesRead;
                                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    dest.Write(buffer, 0, bytesRead);
                                    dest.Flush();
                                }

                                dest.Close();
                            }

                            source.Close();
                        }

                        txn.Commit();
                    }
                }
            }
        }

        public class AudioLoaded : INotification
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
        }
    }
}