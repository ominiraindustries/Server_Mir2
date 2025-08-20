using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySqlConnector;

namespace Server.MirDatabase.Repositories
{
    public class CharacterMagicRepository
    {
        private readonly string _connectionString;

        public CharacterMagicRepository(string host, int port, string database, string user, string password, bool sslRequired)
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

        public List<UserMagic> LoadMagicsByCharacter(int characterId)
        {
            var list = new List<UserMagic>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT MagicId, Level, Experience, `Key` FROM CharacterMagics WHERE CharacterId = @CharacterId";
            cmd.Parameters.AddWithValue("@CharacterId", characterId);

            using var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
            while (reader.Read())
            {
                var magicId = reader.GetInt32(reader.GetOrdinal("MagicId"));
                var level = reader.GetInt32(reader.GetOrdinal("Level"));
                var exp = reader.GetInt64(reader.GetOrdinal("Experience"));
                var key = reader.IsDBNull(reader.GetOrdinal("Key")) ? (byte)0 : reader.GetByte(reader.GetOrdinal("Key"));

                var um = new UserMagic((Spell)magicId)
                {
                    Level = (byte)Math.Max(0, Math.Min(255, level)),
                    Experience = (ushort)Math.Max(0, Math.Min(ushort.MaxValue, exp)),
                    Key = key,
                    IsTempSpell = false,
                    CastTime = 0
                };
                list.Add(um);
            }
            return list;
        }

        public void SaveMagics(int characterId, IList<UserMagic> magics)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            using (var del = conn.CreateCommand())
            {
                del.Transaction = tx;
                del.CommandText = "DELETE FROM CharacterMagics WHERE CharacterId = @CharacterId";
                del.Parameters.AddWithValue("@CharacterId", characterId);
                del.ExecuteNonQuery();
            }

            if (magics != null)
            {
                foreach (var m in magics)
                {
                    using var ins = conn.CreateCommand();
                    ins.Transaction = tx;
                    ins.CommandText = @"INSERT INTO CharacterMagics (CharacterId, MagicId, Level, Experience, `Key`) VALUES (@CharacterId, @MagicId, @Level, @Experience, @Key)";
                    ins.Parameters.AddWithValue("@CharacterId", characterId);
                    ins.Parameters.AddWithValue("@MagicId", (int)m.Spell);
                    ins.Parameters.AddWithValue("@Level", (int)m.Level);
                    ins.Parameters.AddWithValue("@Experience", (long)m.Experience);
                    ins.Parameters.AddWithValue("@Key", (int)m.Key);
                    ins.ExecuteNonQuery();
                }
            }

            tx.Commit();
        }
    }
}
