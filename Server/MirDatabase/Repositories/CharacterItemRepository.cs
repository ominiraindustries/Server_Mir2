using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySqlConnector;

namespace Server.MirDatabase.Repositories
{
public enum ItemContainer : byte
{
    Inventory = 0,
    Equipment = 1,
    QuestInventory = 2
}

public class CharacterItemRepository
{
    private readonly string _connectionString;

    public CharacterItemRepository(string host, int port, string database, string user, string password, bool sslRequired)
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

    public (UserItem[] Inventory, UserItem[] Equipment, UserItem[] QuestInventory) LoadItemsByCharacter(int characterId)
    {
        var inv = new UserItem[46];
        var eq = new UserItem[14];
        var quest = new UserItem[40];

        using var conn = new MySqlConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT Container, Slot, ItemData FROM CharacterItems WHERE CharacterId = @CharacterId";
        cmd.Parameters.AddWithValue("@CharacterId", characterId);

        using var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
        while (reader.Read())
        {
            var container = (ItemContainer)reader.GetByte(reader.GetOrdinal("Container"));
            var slot = reader.GetInt32(reader.GetOrdinal("Slot"));
            using var ms = new MemoryStream();
            using (var blob = reader.GetStream(reader.GetOrdinal("ItemData")))
            {
                blob.CopyTo(ms);
            }
            ms.Position = 0;
            using var br = new BinaryReader(ms);
            var item = new UserItem(br);

            switch (container)
            {
                case ItemContainer.Inventory:
                    if (slot >= 0 && slot < inv.Length) inv[slot] = item;
                    break;
                case ItemContainer.Equipment:
                    if (slot >= 0 && slot < eq.Length) eq[slot] = item;
                    break;
                case ItemContainer.QuestInventory:
                    if (slot >= 0 && slot < quest.Length) quest[slot] = item;
                    break;
            }
        }

        return (inv, eq, quest);
    }

    public void SaveItems(int characterId, UserItem[] inventory, UserItem[] equipment, UserItem[] questInventory)
    {
        using var conn = new MySqlConnection(_connectionString);
        conn.Open();
        using var tx = conn.BeginTransaction();

        using (var del = conn.CreateCommand())
        {
            del.Transaction = tx;
            del.CommandText = "DELETE FROM CharacterItems WHERE CharacterId = @CharacterId";
            del.Parameters.AddWithValue("@CharacterId", characterId);
            del.ExecuteNonQuery();
        }

        void InsertArray(UserItem[] arr, ItemContainer container)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                var item = arr[i];
                if (item == null) continue;

                using var ms = new MemoryStream();
                using (var bw = new BinaryWriter(ms))
                {
                    item.Save(bw);
                    bw.Flush();
                }
                var data = ms.ToArray();

                using var ins = conn.CreateCommand();
                ins.Transaction = tx;
                ins.CommandText = @"INSERT INTO CharacterItems
(CharacterId, Container, Slot, ItemIndex, UniqueID, ItemData)
VALUES (@CharacterId, @Container, @Slot, @ItemIndex, @UniqueID, @ItemData)";
                ins.Parameters.AddWithValue("@CharacterId", characterId);
                ins.Parameters.AddWithValue("@Container", (byte)container);
                ins.Parameters.AddWithValue("@Slot", i);
                ins.Parameters.AddWithValue("@ItemIndex", item.ItemIndex);
                var pUid = ins.Parameters.Add("@UniqueID", MySqlDbType.UInt64);
                pUid.Value = item.UniqueID;
                ins.Parameters.Add("@ItemData", MySqlDbType.LongBlob).Value = data;
                ins.ExecuteNonQuery();
            }
        }

        InsertArray(inventory ?? Array.Empty<UserItem>(), ItemContainer.Inventory);
        InsertArray(equipment ?? Array.Empty<UserItem>(), ItemContainer.Equipment);
        InsertArray(questInventory ?? Array.Empty<UserItem>(), ItemContainer.QuestInventory);

        tx.Commit();
    }
}

}
