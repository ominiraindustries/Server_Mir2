using System;
using System.Collections.Generic;
using MySqlConnector;
using Server.MirDatabase;
using System.Data;

namespace Server.MirDatabase.Repositories
{
    public class AccountRepository
    {
        private readonly string _connectionString;

        public AccountRepository(
            string host,
            int port,
            string database,
            string user,
            string password,
            bool sslRequired)
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

        public List<AccountInfo> LoadAccounts()
        {
            var list = new List<AccountInfo>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT 
                    Id,
                    AccountID,
                    PasswordHash,
                    Salt,
                    RequirePasswordChange,
                    UserName,
                    BirthDate,
                    SecretQuestion,
                    SecretAnswer,
                    Email,
                    CreationIP,
                    CreationDate,
                    Banned,
                    BanReason,
                    ExpiryDate,
                    LastIP,
                    LastDate,
                    HasExpandedStorage,
                    ExpandedStorageExpiryDate,
                    Gold,
                    Credit,
                    AdminAccount,
                    WrongPasswordCount
                FROM Accounts";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var acc = new AccountInfo
                {
                    Index = reader.GetInt32(reader.GetOrdinal("Id")),
                    AccountID = reader.GetString(reader.GetOrdinal("AccountID")),
                    RequirePasswordChange = reader.GetBoolean(reader.GetOrdinal("RequirePasswordChange")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName")),
                    SecretQuestion = reader.IsDBNull(reader.GetOrdinal("SecretQuestion")) ? string.Empty : reader.GetString(reader.GetOrdinal("SecretQuestion")),
                    SecretAnswer = reader.IsDBNull(reader.GetOrdinal("SecretAnswer")) ? string.Empty : reader.GetString(reader.GetOrdinal("SecretAnswer")),
                    EMailAddress = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                    CreationIP = reader.GetString(reader.GetOrdinal("CreationIP")),
                    Banned = reader.GetBoolean(reader.GetOrdinal("Banned")),
                    BanReason = reader.IsDBNull(reader.GetOrdinal("BanReason")) ? string.Empty : reader.GetString(reader.GetOrdinal("BanReason")),
                    LastIP = reader.IsDBNull(reader.GetOrdinal("LastIP")) ? string.Empty : reader.GetString(reader.GetOrdinal("LastIP")),
                    HasExpandedStorage = reader.GetBoolean(reader.GetOrdinal("HasExpandedStorage")),
                    Gold = reader.GetFieldValue<uint>(reader.GetOrdinal("Gold")),
                    Credit = reader.GetFieldValue<uint>(reader.GetOrdinal("Credit")),
                    AdminAccount = reader.GetBoolean(reader.GetOrdinal("AdminAccount")),
                    WrongPasswordCount = reader.GetInt32(reader.GetOrdinal("WrongPasswordCount"))
                };

                // Dates
                acc.BirthDate = reader.IsDBNull(reader.GetOrdinal("BirthDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("BirthDate"));
                acc.CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate"));
                acc.ExpiryDate = reader.IsDBNull(reader.GetOrdinal("ExpiryDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("ExpiryDate"));
                acc.LastDate = reader.IsDBNull(reader.GetOrdinal("LastDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("LastDate"));
                acc.ExpandedStorageExpiryDate = reader.IsDBNull(reader.GetOrdinal("ExpandedStorageExpiryDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("ExpandedStorageExpiryDate"));

                // Password + salt
                var hash = reader.GetString(reader.GetOrdinal("PasswordHash"));
                var salt = (byte[])reader["Salt"];
                acc.SetPasswordHashAndSalt(hash, salt);

                list.Add(acc);
            }

            return list;
        }

        public int CreateAccount(AccountInfo acc)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Accounts
                (AccountID, PasswordHash, Salt, RequirePasswordChange, UserName, BirthDate, SecretQuestion, SecretAnswer, Email,
                 CreationIP, CreationDate, Banned, BanReason, ExpiryDate, LastIP, LastDate, HasExpandedStorage, ExpandedStorageExpiryDate,
                 Gold, Credit, AdminAccount, WrongPasswordCount)
                VALUES
                (@AccountID, @PasswordHash, @Salt, @RequirePasswordChange, @UserName, @BirthDate, @SecretQuestion, @SecretAnswer, @Email,
                 @CreationIP, @CreationDate, @Banned, @BanReason, @ExpiryDate, @LastIP, @LastDate, @HasExpandedStorage, @ExpandedStorageExpiryDate,
                 @Gold, @Credit, @AdminAccount, @WrongPasswordCount); SELECT LAST_INSERT_ID();";

            BindAccountParams(cmd, acc);

            var idObj = cmd.ExecuteScalar();
            return Convert.ToInt32(idObj);
        }

        public void UpsertAccounts(IEnumerable<AccountInfo> accounts)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();

            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"INSERT INTO Accounts
                (Id, AccountID, PasswordHash, Salt, RequirePasswordChange, UserName, BirthDate, SecretQuestion, SecretAnswer, Email,
                 CreationIP, CreationDate, Banned, BanReason, ExpiryDate, LastIP, LastDate, HasExpandedStorage, ExpandedStorageExpiryDate,
                 Gold, Credit, AdminAccount, WrongPasswordCount)
                VALUES
                (@Id, @AccountID, @PasswordHash, @Salt, @RequirePasswordChange, @UserName, @BirthDate, @SecretQuestion, @SecretAnswer, @Email,
                 @CreationIP, @CreationDate, @Banned, @BanReason, @ExpiryDate, @LastIP, @LastDate, @HasExpandedStorage, @ExpandedStorageExpiryDate,
                 @Gold, @Credit, @AdminAccount, @WrongPasswordCount)
                ON DUPLICATE KEY UPDATE
                    PasswordHash = VALUES(PasswordHash),
                    Salt = VALUES(Salt),
                    RequirePasswordChange = VALUES(RequirePasswordChange),
                    UserName = VALUES(UserName),
                    BirthDate = VALUES(BirthDate),
                    SecretQuestion = VALUES(SecretQuestion),
                    SecretAnswer = VALUES(SecretAnswer),
                    Email = VALUES(Email),
                    CreationIP = VALUES(CreationIP),
                    CreationDate = VALUES(CreationDate),
                    Banned = VALUES(Banned),
                    BanReason = VALUES(BanReason),
                    ExpiryDate = VALUES(ExpiryDate),
                    LastIP = VALUES(LastIP),
                    LastDate = VALUES(LastDate),
                    HasExpandedStorage = VALUES(HasExpandedStorage),
                    ExpandedStorageExpiryDate = VALUES(ExpandedStorageExpiryDate),
                    Gold = VALUES(Gold),
                    Credit = VALUES(Credit),
                    AdminAccount = VALUES(AdminAccount),
                    WrongPasswordCount = VALUES(WrongPasswordCount);";

            // Pre-create parameters
            cmd.Parameters.Add("@Id", MySqlDbType.Int32);
            cmd.Parameters.Add("@AccountID", MySqlDbType.VarChar);
            cmd.Parameters.Add("@PasswordHash", MySqlDbType.VarChar);
            cmd.Parameters.Add("@Salt", MySqlDbType.Blob);
            cmd.Parameters.Add("@RequirePasswordChange", MySqlDbType.Bool);
            cmd.Parameters.Add("@UserName", MySqlDbType.VarChar);
            cmd.Parameters.Add("@BirthDate", MySqlDbType.DateTime);
            cmd.Parameters.Add("@SecretQuestion", MySqlDbType.VarChar);
            cmd.Parameters.Add("@SecretAnswer", MySqlDbType.VarChar);
            cmd.Parameters.Add("@Email", MySqlDbType.VarChar);
            cmd.Parameters.Add("@CreationIP", MySqlDbType.VarChar);
            cmd.Parameters.Add("@CreationDate", MySqlDbType.DateTime);
            cmd.Parameters.Add("@Banned", MySqlDbType.Bool);
            cmd.Parameters.Add("@BanReason", MySqlDbType.VarChar);
            cmd.Parameters.Add("@ExpiryDate", MySqlDbType.DateTime);
            cmd.Parameters.Add("@LastIP", MySqlDbType.VarChar);
            cmd.Parameters.Add("@LastDate", MySqlDbType.DateTime);
            cmd.Parameters.Add("@HasExpandedStorage", MySqlDbType.Bool);
            cmd.Parameters.Add("@ExpandedStorageExpiryDate", MySqlDbType.DateTime);
            cmd.Parameters.Add("@Gold", MySqlDbType.UInt32);
            cmd.Parameters.Add("@Credit", MySqlDbType.UInt32);
            cmd.Parameters.Add("@AdminAccount", MySqlDbType.Bool);
            cmd.Parameters.Add("@WrongPasswordCount", MySqlDbType.Int32);

            foreach (var acc in accounts)
            {
                cmd.Parameters["@Id"].Value = acc.Index;
                cmd.Parameters["@AccountID"].Value = acc.AccountID ?? string.Empty;
                cmd.Parameters["@PasswordHash"].Value = acc.Password ?? string.Empty;
                cmd.Parameters["@Salt"].Value = acc.Salt ?? Array.Empty<byte>();
                cmd.Parameters["@RequirePasswordChange"].Value = acc.RequirePasswordChange;
                cmd.Parameters["@UserName"].Value = acc.UserName ?? string.Empty;
                cmd.Parameters["@BirthDate"].Value = acc.BirthDate == DateTime.MinValue ? (object)DBNull.Value : acc.BirthDate;
                cmd.Parameters["@SecretQuestion"].Value = acc.SecretQuestion ?? string.Empty;
                cmd.Parameters["@SecretAnswer"].Value = acc.SecretAnswer ?? string.Empty;
                cmd.Parameters["@Email"].Value = acc.EMailAddress ?? string.Empty;
                cmd.Parameters["@CreationIP"].Value = acc.CreationIP ?? string.Empty;
                cmd.Parameters["@CreationDate"].Value = acc.CreationDate == DateTime.MinValue ? DateTime.UtcNow : acc.CreationDate;
                cmd.Parameters["@Banned"].Value = acc.Banned;
                cmd.Parameters["@BanReason"].Value = acc.BanReason ?? string.Empty;
                cmd.Parameters["@ExpiryDate"].Value = acc.ExpiryDate == DateTime.MinValue ? (object)DBNull.Value : acc.ExpiryDate;
                cmd.Parameters["@LastIP"].Value = acc.LastIP ?? string.Empty;
                cmd.Parameters["@LastDate"].Value = acc.LastDate == DateTime.MinValue ? (object)DBNull.Value : acc.LastDate;
                cmd.Parameters["@HasExpandedStorage"].Value = acc.HasExpandedStorage;
                cmd.Parameters["@ExpandedStorageExpiryDate"].Value = acc.ExpandedStorageExpiryDate == DateTime.MinValue ? (object)DBNull.Value : acc.ExpandedStorageExpiryDate;
                cmd.Parameters["@Gold"].Value = acc.Gold;
                cmd.Parameters["@Credit"].Value = acc.Credit;
                cmd.Parameters["@AdminAccount"].Value = acc.AdminAccount;
                cmd.Parameters["@WrongPasswordCount"].Value = acc.WrongPasswordCount;

                cmd.ExecuteNonQuery();
            }

            tx.Commit();
        }

        private static void BindAccountParams(MySqlCommand cmd, AccountInfo acc)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@AccountID", acc.AccountID ?? string.Empty);
            cmd.Parameters.AddWithValue("@PasswordHash", acc.Password ?? string.Empty);
            cmd.Parameters.AddWithValue("@Salt", acc.Salt ?? Array.Empty<byte>());
            cmd.Parameters.AddWithValue("@RequirePasswordChange", acc.RequirePasswordChange);
            cmd.Parameters.AddWithValue("@UserName", acc.UserName ?? string.Empty);
            cmd.Parameters.AddWithValue("@BirthDate", acc.BirthDate == DateTime.MinValue ? (object)DBNull.Value : acc.BirthDate);
            cmd.Parameters.AddWithValue("@SecretQuestion", acc.SecretQuestion ?? string.Empty);
            cmd.Parameters.AddWithValue("@SecretAnswer", acc.SecretAnswer ?? string.Empty);
            cmd.Parameters.AddWithValue("@Email", acc.EMailAddress ?? string.Empty);
            cmd.Parameters.AddWithValue("@CreationIP", acc.CreationIP ?? string.Empty);
            cmd.Parameters.AddWithValue("@CreationDate", acc.CreationDate == DateTime.MinValue ? DateTime.UtcNow : acc.CreationDate);
            cmd.Parameters.AddWithValue("@Banned", acc.Banned);
            cmd.Parameters.AddWithValue("@BanReason", acc.BanReason ?? string.Empty);
            cmd.Parameters.AddWithValue("@ExpiryDate", acc.ExpiryDate == DateTime.MinValue ? (object)DBNull.Value : acc.ExpiryDate);
            cmd.Parameters.AddWithValue("@LastIP", acc.LastIP ?? string.Empty);
            cmd.Parameters.AddWithValue("@LastDate", acc.LastDate == DateTime.MinValue ? (object)DBNull.Value : acc.LastDate);
            cmd.Parameters.AddWithValue("@HasExpandedStorage", acc.HasExpandedStorage);
            cmd.Parameters.AddWithValue("@ExpandedStorageExpiryDate", acc.ExpandedStorageExpiryDate == DateTime.MinValue ? (object)DBNull.Value : acc.ExpandedStorageExpiryDate);
            cmd.Parameters.AddWithValue("@Gold", acc.Gold);
            cmd.Parameters.AddWithValue("@Credit", acc.Credit);
            cmd.Parameters.AddWithValue("@AdminAccount", acc.AdminAccount);
            cmd.Parameters.AddWithValue("@WrongPasswordCount", acc.WrongPasswordCount);
        }
    }
}
