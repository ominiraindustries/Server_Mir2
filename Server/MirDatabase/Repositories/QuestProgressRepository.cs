using System.Collections.Generic;
using System.Data;
using System.IO;
using MySqlConnector;

#nullable enable

namespace Server.MirDatabase.Repositories
{
    public class QuestProgressRepository
    {
        private readonly string _connectionString;

        public QuestProgressRepository(string host, int port, string database, string user, string password, bool sslRequired)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = host,
                Port = (uint)port,
                Database = database,
                UserID = user,
                Password = password,
                SslMode = sslRequired ? MySqlSslMode.Required : MySqlSslMode.None,
                ConnectionTimeout = 5,
                DefaultCommandTimeout = 30,
                AllowUserVariables = true
            };
            _connectionString = builder.ConnectionString;
        }

        public List<(int QuestId, byte State, byte[]? Data)> LoadByCharacter(int characterId)
        {
            var list = new List<(int, byte, byte[]?)>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT QuestId, State, ProgressData FROM QuestProgress WHERE CharacterId = @CharacterId";
            cmd.Parameters.AddWithValue("@CharacterId", characterId);
            using var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
            while (reader.Read())
            {
                int questId = reader.GetInt32(reader.GetOrdinal("QuestId"));
                byte state = reader.GetByte(reader.GetOrdinal("State"));
                byte[]? data = null;
                if (!reader.IsDBNull(reader.GetOrdinal("ProgressData")))
                {
                    using var ms = new MemoryStream();
                    using (var blob = reader.GetStream(reader.GetOrdinal("ProgressData"))) { blob.CopyTo(ms); }
                    data = ms.ToArray();
                }
                list.Add((questId, state, data));
            }
            return list;
        }

        public void Upsert(int characterId, int questId, byte state, byte[]? progressData)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO QuestProgress (CharacterId, QuestId, State, ProgressData)
VALUES (@CharacterId, @QuestId, @State, @ProgressData)
ON DUPLICATE KEY UPDATE State = VALUES(State), ProgressData = VALUES(ProgressData)";
            cmd.Parameters.AddWithValue("@CharacterId", characterId);
            cmd.Parameters.AddWithValue("@QuestId", questId);
            cmd.Parameters.AddWithValue("@State", state);
            if (progressData == null) cmd.Parameters.AddWithValue("@ProgressData", DBNull.Value); else cmd.Parameters.Add("@ProgressData", MySqlDbType.LongBlob).Value = progressData;
            cmd.ExecuteNonQuery();
        }

        public void Delete(int characterId, int questId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM QuestProgress WHERE CharacterId = @CharacterId AND QuestId = @QuestId";
            cmd.Parameters.AddWithValue("@CharacterId", characterId);
            cmd.Parameters.AddWithValue("@QuestId", questId);
            cmd.ExecuteNonQuery();
        }
    }
}
