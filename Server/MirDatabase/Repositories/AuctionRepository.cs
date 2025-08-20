using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySqlConnector;

namespace Server.MirDatabase.Repositories
{
    public class AuctionRecord
    {
        public long Id { get; set; }
        public int SellerCharacterId { get; set; }
        public int? BuyerCharacterId { get; set; }
        public ulong? FinalPrice { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserItem Item { get; set; } = default!;
    }

    public class AuctionRepository
    {
        private readonly string _connectionString;

        public AuctionRepository(string host, int port, string database, string user, string password, bool sslRequired)
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

        public List<AuctionRecord> GetActive()
        {
            var list = new List<AuctionRecord>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Id, SellerCharacterId, BuyerCharacterId, FinalPrice, Status, CreatedAt, ExpiresAt, ItemData
FROM Auctions WHERE Status = 0 AND ExpiresAt >= CURRENT_TIMESTAMP";
            using var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
            while (reader.Read())
            {
                var rec = new AuctionRecord
                {
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                    SellerCharacterId = reader.GetInt32(reader.GetOrdinal("SellerCharacterId")),
                    BuyerCharacterId = reader.IsDBNull(reader.GetOrdinal("BuyerCharacterId")) ? null : reader.GetInt32(reader.GetOrdinal("BuyerCharacterId")),
                    FinalPrice = reader.IsDBNull(reader.GetOrdinal("FinalPrice")) ? null : reader.GetFieldValue<ulong>(reader.GetOrdinal("FinalPrice")),
                    Status = reader.GetByte(reader.GetOrdinal("Status")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    ExpiresAt = reader.GetDateTime(reader.GetOrdinal("ExpiresAt"))
                };
                using var ms = new MemoryStream();
                using (var blob = reader.GetStream(reader.GetOrdinal("ItemData"))) { blob.CopyTo(ms); }
                ms.Position = 0;
                using var br = new BinaryReader(ms);
                rec.Item = new UserItem(br);
                list.Add(rec);
            }
            return list;
        }

        public long Create(int sellerCharacterId, UserItem item, ulong startPrice, ulong? buyoutPrice, DateTime expiresAt)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            using var ms = new MemoryStream();
            using (var bw = new BinaryWriter(ms)) { item.Save(bw); bw.Flush(); }
            var data = ms.ToArray();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Auctions (SellerCharacterId, ItemIndex, UniqueID, ItemData, StartPrice, BuyoutPrice, ExpiresAt)
VALUES (@Seller, @ItemIndex, @UniqueID, @ItemData, @StartPrice, @BuyoutPrice, @ExpiresAt);
SELECT LAST_INSERT_ID();";
            cmd.Parameters.AddWithValue("@Seller", sellerCharacterId);
            cmd.Parameters.AddWithValue("@ItemIndex", item.ItemIndex);
            var pUid = cmd.Parameters.Add("@UniqueID", MySqlDbType.UInt64); pUid.Value = item.UniqueID;
            cmd.Parameters.Add("@ItemData", MySqlDbType.LongBlob).Value = data;
            var pStart = cmd.Parameters.Add("@StartPrice", MySqlDbType.UInt64); pStart.Value = startPrice;
            if (buyoutPrice.HasValue) { var p = cmd.Parameters.Add("@BuyoutPrice", MySqlDbType.UInt64); p.Value = buyoutPrice.Value; }
            else cmd.Parameters.AddWithValue("@BuyoutPrice", DBNull.Value);
            cmd.Parameters.AddWithValue("@ExpiresAt", expiresAt);

            var idObj = cmd.ExecuteScalar();
            return Convert.ToInt64(idObj);
        }

        public void MarkSold(long auctionId, int buyerCharacterId, ulong finalPrice)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Auctions SET BuyerCharacterId = @Buyer, FinalPrice = @Final, Status = 1 WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", auctionId);
            cmd.Parameters.AddWithValue("@Buyer", buyerCharacterId);
            var p = cmd.Parameters.Add("@Final", MySqlDbType.UInt64); p.Value = finalPrice;
            cmd.ExecuteNonQuery();
        }

        public void Cancel(long auctionId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Auctions SET Status = 2 WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", auctionId);
            cmd.ExecuteNonQuery();
        }

        public void Expire(long auctionId)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Auctions SET Status = 3 WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", auctionId);
            cmd.ExecuteNonQuery();
        }
    }
}
