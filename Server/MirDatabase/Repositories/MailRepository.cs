using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySqlConnector;

#nullable enable

namespace Server.MirDatabase.Repositories
{
    public class MailHeader
    {
        public long Id { get; set; }
        public int CharacterId { get; set; }
        public string Sender { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string? Body { get; set; }
        public uint Gold { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? ClaimedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    public class MailRepository
    {
        private readonly string _connectionString;

        public MailRepository(string host, int port, string database, string user, string password, bool sslRequired)
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

        public List<MailHeader> GetMailByCharacter(int characterId)
        {
            var list = new List<MailHeader>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, CharacterId, Sender, Subject, Body, Gold, CreatedAt, ReadAt, ClaimedAt, DeletedAt
FROM Mail WHERE CharacterId = @CharacterId AND (DeletedAt IS NULL) ORDER BY CreatedAt DESC";
            cmd.Parameters.AddWithValue("@CharacterId", characterId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new MailHeader
                {
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                    CharacterId = reader.GetInt32(reader.GetOrdinal("CharacterId")),
                    Sender = reader.GetString(reader.GetOrdinal("Sender")),
                    Subject = reader.GetString(reader.GetOrdinal("Subject")),
                    Body = reader.IsDBNull(reader.GetOrdinal("Body")) ? null : reader.GetString(reader.GetOrdinal("Body")),
                    Gold = reader.GetFieldValue<uint>(reader.GetOrdinal("Gold")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    ReadAt = reader.IsDBNull(reader.GetOrdinal("ReadAt")) ? null : reader.GetDateTime(reader.GetOrdinal("ReadAt")),
                    ClaimedAt = reader.IsDBNull(reader.GetOrdinal("ClaimedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("ClaimedAt")),
                    DeletedAt = reader.IsDBNull(reader.GetOrdinal("DeletedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("DeletedAt"))
                });
            }
            return list;
        }

        public List<(int Slot, UserItem Item)> GetMailItems(long mailId)
        {
            var list = new List<(int, UserItem)>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Slot, ItemData FROM MailItems WHERE MailId = @MailId ORDER BY Slot";
            cmd.Parameters.AddWithValue("@MailId", mailId);
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
                list.Add((slot, item));
            }
            return list;
        }

        public long CreateMail(int characterId, string sender, string subject, string? body, uint gold, IEnumerable<(int Slot, UserItem Item)>? attachments)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            long mailId;
            using (var ins = conn.CreateCommand())
            {
                ins.Transaction = tx;
                ins.CommandText = @"INSERT INTO Mail (CharacterId, Sender, Subject, Body, Gold) VALUES (@CharacterId, @Sender, @Subject, @Body, @Gold); SELECT LAST_INSERT_ID();";
                ins.Parameters.AddWithValue("@CharacterId", characterId);
                ins.Parameters.AddWithValue("@Sender", sender);
                ins.Parameters.AddWithValue("@Subject", subject);
                if (body == null) ins.Parameters.AddWithValue("@Body", DBNull.Value); else ins.Parameters.AddWithValue("@Body", body);
                var pGold = ins.Parameters.Add("@Gold", MySqlDbType.UInt32); pGold.Value = gold;
                mailId = Convert.ToInt64(ins.ExecuteScalar());
            }

            if (attachments != null)
            {
                foreach (var att in attachments)
                {
                    if (att.Item == null) continue;
                    using var ms = new MemoryStream();
                    using (var bw = new BinaryWriter(ms)) { att.Item.Save(bw); bw.Flush(); }
                    var data = ms.ToArray();

                    using var insI = conn.CreateCommand();
                    insI.Transaction = tx;
                    insI.CommandText = @"INSERT INTO MailItems (MailId, Slot, ItemIndex, UniqueID, ItemData) VALUES (@MailId, @Slot, @ItemIndex, @UniqueID, @ItemData)";
                    insI.Parameters.AddWithValue("@MailId", mailId);
                    insI.Parameters.AddWithValue("@Slot", att.Slot);
                    insI.Parameters.AddWithValue("@ItemIndex", att.Item.ItemIndex);
                    var pUid = insI.Parameters.Add("@UniqueID", MySqlDbType.UInt64); pUid.Value = att.Item.UniqueID;
                    insI.Parameters.Add("@ItemData", MySqlDbType.LongBlob).Value = data;
                    insI.ExecuteNonQuery();
                }
            }

            tx.Commit();
            return mailId;
        }

        public void MarkRead(long mailId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Mail SET ReadAt = CURRENT_TIMESTAMP WHERE Id = @Id AND ReadAt IS NULL";
            cmd.Parameters.AddWithValue("@Id", mailId);
            cmd.ExecuteNonQuery();
        }

        public void MarkClaimed(long mailId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Mail SET ClaimedAt = CURRENT_TIMESTAMP WHERE Id = @Id AND ClaimedAt IS NULL";
            cmd.Parameters.AddWithValue("@Id", mailId);
            cmd.ExecuteNonQuery();
        }

        public void SoftDelete(long mailId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Mail SET DeletedAt = CURRENT_TIMESTAMP WHERE Id = @Id AND DeletedAt IS NULL";
            cmd.Parameters.AddWithValue("@Id", mailId);
            cmd.ExecuteNonQuery();
        }
    }
}
