using System.Collections.Generic;
using System.Data;
using MySqlConnector;

namespace Server.MirDatabase.Repositories
{
    public class FriendsBlocksRepository
    {
        private readonly string _connectionString;

        public FriendsBlocksRepository(string host, int port, string database, string user, string password, bool sslRequired)
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

        // Friends
        public List<int> GetFriends(int characterId)
        {
            var list = new List<int>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT FriendCharacterId FROM Friends WHERE CharacterId = @CharacterId";
            cmd.Parameters.AddWithValue("@CharacterId", characterId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(reader.GetInt32(0));
            return list;
        }

        public void AddFriend(int characterId, int friendCharacterId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT IGNORE INTO Friends (CharacterId, FriendCharacterId) VALUES (@C, @F)";
            cmd.Parameters.AddWithValue("@C", characterId);
            cmd.Parameters.AddWithValue("@F", friendCharacterId);
            cmd.ExecuteNonQuery();
        }

        public void RemoveFriend(int characterId, int friendCharacterId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Friends WHERE CharacterId = @C AND FriendCharacterId = @F";
            cmd.Parameters.AddWithValue("@C", characterId);
            cmd.Parameters.AddWithValue("@F", friendCharacterId);
            cmd.ExecuteNonQuery();
        }

        // Blocks
        public List<int> GetBlocks(int characterId)
        {
            var list = new List<int>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT BlockCharacterId FROM Blocks WHERE CharacterId = @CharacterId";
            cmd.Parameters.AddWithValue("@CharacterId", characterId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(reader.GetInt32(0));
            return list;
        }

        public void AddBlock(int characterId, int blockedCharacterId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT IGNORE INTO Blocks (CharacterId, BlockCharacterId) VALUES (@C, @B)";
            cmd.Parameters.AddWithValue("@C", characterId);
            cmd.Parameters.AddWithValue("@B", blockedCharacterId);
            cmd.ExecuteNonQuery();
        }

        public void RemoveBlock(int characterId, int blockedCharacterId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Blocks WHERE CharacterId = @C AND BlockCharacterId = @B";
            cmd.Parameters.AddWithValue("@C", characterId);
            cmd.Parameters.AddWithValue("@B", blockedCharacterId);
            cmd.ExecuteNonQuery();
        }
    }
}
