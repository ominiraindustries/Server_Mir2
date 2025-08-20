using System;
using System.Data;
using MySqlConnector;

#nullable enable

namespace Server.MirDatabase.Repositories
{
    public class GuildRecord
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? LeaderCharacterId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ulong Gold { get; set; }
    }

    public class GuildRepository
    {
        private readonly string _connectionString;

        public GuildRepository(string host, int port, string database, string user, string password, bool sslRequired)
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

        public GuildRecord? GetById(int guildId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, LeaderCharacterId, CreatedAt, Gold FROM Guilds WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", guildId);

            using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
            if (!reader.Read()) return null;

            return new GuildRecord
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                LeaderCharacterId = reader.IsDBNull(reader.GetOrdinal("LeaderCharacterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("LeaderCharacterId")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                Gold = reader.GetFieldValue<ulong>(reader.GetOrdinal("Gold"))
            };
        }

        public GuildRecord? GetByName(string name)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, LeaderCharacterId, CreatedAt, Gold FROM Guilds WHERE Name = @Name";
            cmd.Parameters.AddWithValue("@Name", name);

            using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
            if (!reader.Read()) return null;

            return new GuildRecord
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                LeaderCharacterId = reader.IsDBNull(reader.GetOrdinal("LeaderCharacterId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("LeaderCharacterId")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                Gold = reader.GetFieldValue<ulong>(reader.GetOrdinal("Gold"))
            };
        }

        public int Create(string name, int? leaderCharacterId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Guilds (Name, LeaderCharacterId) VALUES (@Name, @Leader); SELECT LAST_INSERT_ID();";
            cmd.Parameters.AddWithValue("@Name", name);
            if (leaderCharacterId.HasValue)
                cmd.Parameters.AddWithValue("@Leader", leaderCharacterId.Value);
            else
                cmd.Parameters.AddWithValue("@Leader", DBNull.Value);

            var idObj = cmd.ExecuteScalar();
            return Convert.ToInt32(idObj);
        }

        public void UpdateLeader(int guildId, int? leaderCharacterId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Guilds SET LeaderCharacterId = @Leader WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", guildId);
            if (leaderCharacterId.HasValue)
                cmd.Parameters.AddWithValue("@Leader", leaderCharacterId.Value);
            else
                cmd.Parameters.AddWithValue("@Leader", DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public void UpdateGold(int guildId, ulong gold)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Guilds SET Gold = @Gold WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", guildId);
            var pGold = cmd.Parameters.Add("@Gold", MySqlDbType.UInt64);
            pGold.Value = gold;
            cmd.ExecuteNonQuery();
        }
    }
}
