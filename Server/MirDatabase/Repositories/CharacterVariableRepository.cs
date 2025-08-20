using MySqlConnector;
using System.Data;

namespace Server.MirDatabase.Repositories
{
    public class CharacterVariableRepository
    {
        private readonly string _connectionString;

        public CharacterVariableRepository(string host, int port, string database, string user, string password, bool sslRequired)
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

        public List<KeyValuePair<string, string>> LoadVariablesByCharacter(int characterId)
        {
            var results = new List<KeyValuePair<string, string>>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT VarKey, VarValue FROM CharacterVariables WHERE CharacterId = @CharacterId";
            cmd.Parameters.AddWithValue("@CharacterId", characterId);

            using var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
            while (reader.Read())
            {
                var key = reader.GetString(reader.GetOrdinal("VarKey"));
                string value = reader.IsDBNull(reader.GetOrdinal("VarValue")) ? string.Empty : reader.GetString(reader.GetOrdinal("VarValue"));
                results.Add(new KeyValuePair<string, string>(key, value));
            }

            return results;
        }

        public void SaveVariables(int characterId, IEnumerable<KeyValuePair<string, string>> variables)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            using (var del = conn.CreateCommand())
            {
                del.Transaction = tx;
                del.CommandText = "DELETE FROM CharacterVariables WHERE CharacterId = @CharacterId";
                del.Parameters.AddWithValue("@CharacterId", characterId);
                del.ExecuteNonQuery();
            }

            if (variables != null)
            {
                foreach (var kv in variables)
                {
                    using var ins = conn.CreateCommand();
                    ins.Transaction = tx;
                    ins.CommandText = @"INSERT INTO CharacterVariables (CharacterId, VarKey, VarValue) VALUES (@CharacterId, @VarKey, @VarValue)";
                    ins.Parameters.AddWithValue("@CharacterId", characterId);
                    ins.Parameters.AddWithValue("@VarKey", kv.Key ?? string.Empty);
                    if (kv.Value == null)
                        ins.Parameters.AddWithValue("@VarValue", DBNull.Value);
                    else
                        ins.Parameters.AddWithValue("@VarValue", kv.Value);
                    ins.ExecuteNonQuery();
                }
            }

            tx.Commit();
        }
    }
}
