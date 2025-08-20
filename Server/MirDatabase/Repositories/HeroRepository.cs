using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySqlConnector;

namespace Server.MirDatabase.Repositories
{
    public class HeroRecord
    {
        public long Id { get; set; }
        public int CharacterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public byte Class { get; set; }
        public byte[] HeroData { get; set; } = Array.Empty<byte>();
    }

    public enum HeroItemContainer : byte
    {
        Inventory = 0,
        Equipment = 1,
        QuestInventory = 2
    }

    public class HeroRepository
    {
        private readonly string _connectionString;

        public HeroRepository(string host, int port, string database, string user, string password, bool sslRequired)
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

        public List<HeroRecord> GetByCharacter(int characterId)
        {
            var list = new List<HeroRecord>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, CharacterId, Name, Level, Class, HeroData FROM Heroes WHERE CharacterId = @CharacterId";
            cmd.Parameters.AddWithValue("@CharacterId", characterId);
            using var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
            while (reader.Read())
            {
                var rec = new HeroRecord
                {
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                    CharacterId = reader.GetInt32(reader.GetOrdinal("CharacterId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Level = reader.GetInt32(reader.GetOrdinal("Level")),
                    Class = reader.GetByte(reader.GetOrdinal("Class"))
                };
                using var ms = new MemoryStream();
                using (var blob = reader.GetStream(reader.GetOrdinal("HeroData"))) { blob.CopyTo(ms); }
                rec.HeroData = ms.ToArray();
                list.Add(rec);
            }
            return list;
        }

        public long Create(int characterId, string name, int level, byte @class, byte[] heroData)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Heroes (CharacterId, Name, Level, Class, HeroData)
VALUES (@CharacterId, @Name, @Level, @Class, @HeroData); SELECT LAST_INSERT_ID();";
            cmd.Parameters.AddWithValue("@CharacterId", characterId);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@Level", level);
            cmd.Parameters.AddWithValue("@Class", @class);
            cmd.Parameters.Add("@HeroData", MySqlDbType.LongBlob).Value = heroData ?? Array.Empty<byte>();
            var idObj = cmd.ExecuteScalar();
            return Convert.ToInt64(idObj);
        }

        public void UpdateHeroData(long heroId, byte[] heroData)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Heroes SET HeroData = @HeroData, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", heroId);
            cmd.Parameters.Add("@HeroData", MySqlDbType.LongBlob).Value = heroData ?? Array.Empty<byte>();
            cmd.ExecuteNonQuery();
        }

        public (UserItem[] Inventory, UserItem[] Equipment, UserItem[] QuestInventory) LoadItems(long heroId)
        {
            var inv = new UserItem[46];
            var eq = new UserItem[14];
            var quest = new UserItem[40];

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Container, Slot, ItemData FROM HeroItems WHERE HeroId = @HeroId";
            cmd.Parameters.AddWithValue("@HeroId", heroId);
            using var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
            while (reader.Read())
            {
                var container = (HeroItemContainer)reader.GetByte(reader.GetOrdinal("Container"));
                var slot = reader.GetInt32(reader.GetOrdinal("Slot"));
                using var ms = new MemoryStream();
                using (var blob = reader.GetStream(reader.GetOrdinal("ItemData"))) { blob.CopyTo(ms); }
                ms.Position = 0; using var br = new BinaryReader(ms);
                var item = new UserItem(br);
                switch (container)
                {
                    case HeroItemContainer.Inventory: if (slot >= 0 && slot < inv.Length) inv[slot] = item; break;
                    case HeroItemContainer.Equipment: if (slot >= 0 && slot < eq.Length) eq[slot] = item; break;
                    case HeroItemContainer.QuestInventory: if (slot >= 0 && slot < quest.Length) quest[slot] = item; break;
                }
            }
            return (inv, eq, quest);
        }

        public void SaveItems(long heroId, UserItem[] inventory, UserItem[] equipment, UserItem[] questInventory)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            using (var del = conn.CreateCommand())
            {
                del.Transaction = tx;
                del.CommandText = "DELETE FROM HeroItems WHERE HeroId = @HeroId";
                del.Parameters.AddWithValue("@HeroId", heroId);
                del.ExecuteNonQuery();
            }

            void InsertArray(UserItem[] arr, HeroItemContainer container)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    var item = arr[i];
                    if (item == null) continue;
                    using var ms = new MemoryStream(); using (var bw = new BinaryWriter(ms)) { item.Save(bw); bw.Flush(); }
                    var data = ms.ToArray();
                    using var ins = conn.CreateCommand();
                    ins.Transaction = tx;
                    ins.CommandText = @"INSERT INTO HeroItems (HeroId, Container, Slot, ItemIndex, UniqueID, ItemData)
VALUES (@HeroId, @Container, @Slot, @ItemIndex, @UniqueID, @ItemData)";
                    ins.Parameters.AddWithValue("@HeroId", heroId);
                    ins.Parameters.AddWithValue("@Container", (byte)container);
                    ins.Parameters.AddWithValue("@Slot", i);
                    ins.Parameters.AddWithValue("@ItemIndex", item.ItemIndex);
                    var pUid = ins.Parameters.Add("@UniqueID", MySqlDbType.UInt64); pUid.Value = item.UniqueID;
                    ins.Parameters.Add("@ItemData", MySqlDbType.LongBlob).Value = data;
                    ins.ExecuteNonQuery();
                }
            }

            InsertArray(inventory ?? Array.Empty<UserItem>(), HeroItemContainer.Inventory);
            InsertArray(equipment ?? Array.Empty<UserItem>(), HeroItemContainer.Equipment);
            InsertArray(questInventory ?? Array.Empty<UserItem>(), HeroItemContainer.QuestInventory);

            tx.Commit();
        }
    }
}
