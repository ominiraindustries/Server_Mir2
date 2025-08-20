using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySqlConnector;
using Server.MirEnvir;
using Server.MirDatabase;

namespace Server.MirDatabase.Repositories
{
    public class CharacterBuffRepository
    {
        private readonly string _connectionString;

        public CharacterBuffRepository(string host, int port, string database, string user, string password, bool sslRequired)
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

        public List<Buff> LoadBuffsByCharacter(int characterId)
        {
            var list = new List<Buff>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT BuffData FROM CharacterBuffs WHERE CharacterId = @CharacterId";
            cmd.Parameters.AddWithValue("@CharacterId", characterId);

            using var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
            while (reader.Read())
            {
                using var ms = new MemoryStream();
                using (var blob = reader.GetStream(reader.GetOrdinal("BuffData")))
                {
                    blob.CopyTo(ms);
                }
                ms.Position = 0;
                using var br = new BinaryReader(ms);
                var buff = new Buff(br, Envir.Version, Envir.CustomVersion);
                list.Add(buff);
            }
            return list;
        }

        public void SaveBuffs(int characterId, IList<Buff> buffs)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            using (var del = conn.CreateCommand())
            {
                del.Transaction = tx;
                del.CommandText = "DELETE FROM CharacterBuffs WHERE CharacterId = @CharacterId";
                del.Parameters.AddWithValue("@CharacterId", characterId);
                del.ExecuteNonQuery();
            }

            if (buffs != null)
            {
                foreach (var buff in buffs)
                {
                    using var ms = new MemoryStream();
                    using (var bw = new BinaryWriter(ms))
                    {
                        buff.Save(bw);
                        bw.Flush();
                    }
                    var data = ms.ToArray();

                    using var ins = conn.CreateCommand();
                    ins.Transaction = tx;
                    ins.CommandText = @"INSERT INTO CharacterBuffs (CharacterId, BuffData, ExpiresAt) VALUES (@CharacterId, @BuffData, NULL)";
                    ins.Parameters.AddWithValue("@CharacterId", characterId);
                    ins.Parameters.Add("@BuffData", MySqlDbType.LongBlob).Value = data;
                    ins.ExecuteNonQuery();
                }
            }

            tx.Commit();
        }
    }
}
