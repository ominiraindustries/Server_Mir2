using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySqlConnector;

namespace Server.MirDatabase.Repositories
{
    public class GuildStorageRepository
    {
        private readonly string _connectionString;

        public GuildStorageRepository(string host, int port, string database, string user, string password, bool sslRequired)
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

        public List<(int Slot, UserItem Item)> GetItems(int guildId)
        {
            var result = new List<(int, UserItem)>();

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Slot, ItemData FROM GuildStorageItems WHERE GuildId = @GuildId ORDER BY Slot";
            cmd.Parameters.AddWithValue("@GuildId", guildId);

            using var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
            while (reader.Read())
            {
                int slot = reader.GetInt32(reader.GetOrdinal("Slot"));
                using var ms = new MemoryStream();
                using (var blob = reader.GetStream(reader.GetOrdinal("ItemData")))
                {
                    blob.CopyTo(ms);
                }
                ms.Position = 0;
                using var br = new BinaryReader(ms);
                var item = new UserItem(br);
                result.Add((slot, item));
            }

            return result;
        }

        public void PutItem(int guildId, int slot, UserItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            using var ms = new MemoryStream();
            using (var bw = new BinaryWriter(ms))
            {
                item.Save(bw);
                bw.Flush();
            }
            var data = ms.ToArray();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO GuildStorageItems (GuildId, Slot, ItemIndex, UniqueID, ItemData)
VALUES (@GuildId, @Slot, @ItemIndex, @UniqueID, @ItemData)
ON DUPLICATE KEY UPDATE ItemIndex = VALUES(ItemIndex), UniqueID = VALUES(UniqueID), ItemData = VALUES(ItemData)";
            cmd.Parameters.AddWithValue("@GuildId", guildId);
            cmd.Parameters.AddWithValue("@Slot", slot);
            cmd.Parameters.AddWithValue("@ItemIndex", item.ItemIndex);
            var pUid = cmd.Parameters.Add("@UniqueID", MySqlDbType.UInt64);
            pUid.Value = item.UniqueID;
            cmd.Parameters.Add("@ItemData", MySqlDbType.LongBlob).Value = data;

            cmd.ExecuteNonQuery();
        }

        public void RemoveItem(int guildId, int slot)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM GuildStorageItems WHERE GuildId = @GuildId AND Slot = @Slot";
            cmd.Parameters.AddWithValue("@GuildId", guildId);
            cmd.Parameters.AddWithValue("@Slot", slot);
            cmd.ExecuteNonQuery();
        }

        public void ReplaceAll(int guildId, IEnumerable<(int Slot, UserItem Item)> items)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            using (var del = conn.CreateCommand())
            {
                del.Transaction = tx;
                del.CommandText = "DELETE FROM GuildStorageItems WHERE GuildId = @GuildId";
                del.Parameters.AddWithValue("@GuildId", guildId);
                del.ExecuteNonQuery();
            }

            if (items != null)
            {
                foreach (var entry in items)
                {
                    if (entry.Item == null) continue;

                    using var ms = new MemoryStream();
                    using (var bw = new BinaryWriter(ms))
                    {
                        entry.Item.Save(bw);
                        bw.Flush();
                    }
                    var data = ms.ToArray();

                    using var ins = conn.CreateCommand();
                    ins.Transaction = tx;
                    ins.CommandText = @"INSERT INTO GuildStorageItems (GuildId, Slot, ItemIndex, UniqueID, ItemData)
VALUES (@GuildId, @Slot, @ItemIndex, @UniqueID, @ItemData)";
                    ins.Parameters.AddWithValue("@GuildId", guildId);
                    ins.Parameters.AddWithValue("@Slot", entry.Slot);
                    ins.Parameters.AddWithValue("@ItemIndex", entry.Item.ItemIndex);
                    var pUid = ins.Parameters.Add("@UniqueID", MySqlDbType.UInt64);
                    pUid.Value = entry.Item.UniqueID;
                    ins.Parameters.Add("@ItemData", MySqlDbType.LongBlob).Value = data;
                    ins.ExecuteNonQuery();
                }
            }

            tx.Commit();
        }
    }
}
