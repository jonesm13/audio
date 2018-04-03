namespace Domain.Adapters
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.IO;
    using System.Threading.Tasks;
    using Ports;

    public class SqlFileStreamAudioStore : IAudioStore
    {
        readonly string inboxLocation;

        public SqlFileStreamAudioStore(string inboxLocation)
        {
            this.inboxLocation = inboxLocation;
        }

        public Task StoreAsync(Guid id, string filename)
        {
            string fullFilePath = Path.Combine(inboxLocation, filename);

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
                        cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier)
                            .Value = id;

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            rdr.Read();

                            serverPath = rdr.GetSqlString(0)
                                .Value;
                            serverTxn = rdr.GetSqlBinary(1)
                                .Value;
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
                            while ((bytesRead = source.Read(
                                       buffer,
                                       0,
                                       buffer.Length)) >
                                   0)
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

            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            const string deleteSql = @"DELETE FROM AudioFile WHERE Id = @id;";

            string connectionString = ConfigurationManager
                .ConnectionStrings["audio-db"]
                .ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlTransaction txn = conn.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand(deleteSql, conn))
                    {
                        cmd.Transaction = txn;
                        cmd.Parameters.Add("@id", SqlDbType.UniqueIdentifier)
                            .Value = id;

                        await cmd.ExecuteNonQueryAsync();
                    }

                    txn.Commit();
                }
            }
        }
    }
}
