using System;
using System.Collections.Generic;
using MySqlConnector;
using Server.MirDatabase;
using System.Data;

namespace Server.MirDatabase.Repositories
{
    public class CharacterRepository
    {
        private readonly string _connectionString;

        public CharacterRepository(
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

        public List<CharacterInfo> LoadCharactersByAccount(int accountId)
        {
            var list = new List<CharacterInfo>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT
                    Id, AccountId, Name, Level, Class, Gender, Hair, GuildIndex,
                    CreationIP, CreationDate, Banned, BanReason, ExpiryDate,
                    ChatBanned, ChatBanExpiryDate, LastIP, LastLogoutDate, LastLoginDate,
                    Deleted, DeleteDate,
                    Married, MarriedDate, Mentor, MentorDate, IsMentor, MentorExp,
                    CurrentMapIndex, CurrentLocationX, CurrentLocationY, Direction, BindMapIndex, BindLocationX, BindLocationY,
                    HP, MP, Experience, Gold,
                    AMode, PMode, AllowGroup, AllowTrade, AllowObserve, PKPoints, NewDay,
                    Thrusting, HalfMoon, CrossHalfMoon, DoubleSlash, MentalState, MentalStateLvl,
                    PearlCount, CollectTime
                FROM Characters WHERE AccountId = @AccountId";
            cmd.Parameters.AddWithValue("@AccountId", accountId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(MapCharacter(reader));
            }

            return list;
        }

        public int CreateCharacter(int accountId, CharacterInfo ch)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Characters
                (AccountId, Name, Level, Class, Gender, Hair, GuildIndex,
                 CreationIP, CreationDate, Banned, BanReason, ExpiryDate,
                 ChatBanned, ChatBanExpiryDate, LastIP, LastLogoutDate, LastLoginDate,
                 Deleted, DeleteDate,
                 Married, MarriedDate, Mentor, MentorDate, IsMentor, MentorExp,
                 CurrentMapIndex, CurrentLocationX, CurrentLocationY, Direction, BindMapIndex, BindLocationX, BindLocationY,
                 HP, MP, Experience, Gold,
                 AMode, PMode, AllowGroup, AllowTrade, AllowObserve, PKPoints, NewDay,
                 Thrusting, HalfMoon, CrossHalfMoon, DoubleSlash, MentalState, MentalStateLvl,
                 PearlCount, CollectTime)
                VALUES
                (@AccountId, @Name, @Level, @Class, @Gender, @Hair, @GuildIndex,
                 @CreationIP, @CreationDate, @Banned, @BanReason, @ExpiryDate,
                 @ChatBanned, @ChatBanExpiryDate, @LastIP, @LastLogoutDate, @LastLoginDate,
                 @Deleted, @DeleteDate,
                 @Married, @MarriedDate, @Mentor, @MentorDate, @IsMentor, @MentorExp,
                 @CurrentMapIndex, @CurrentLocationX, @CurrentLocationY, @Direction, @BindMapIndex, @BindLocationX, @BindLocationY,
                 @HP, @MP, @Experience, @Gold,
                 @AMode, @PMode, @AllowGroup, @AllowTrade, @AllowObserve, @PKPoints, @NewDay,
                 @Thrusting, @HalfMoon, @CrossHalfMoon, @DoubleSlash, @MentalState, @MentalStateLvl,
                 @PearlCount, @CollectTime);
                SELECT LAST_INSERT_ID();";

            BindParams(cmd, accountId, ch, includeId:false);

            var idObj = cmd.ExecuteScalar();
            return Convert.ToInt32(idObj);
        }

        public void UpsertCharacters(IEnumerable<(int AccountId, CharacterInfo Ch)> characters)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var tx = conn.BeginTransaction();
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"INSERT INTO Characters
                (Id, AccountId, Name, Level, Class, Gender, Hair, GuildIndex,
                 CreationIP, CreationDate, Banned, BanReason, ExpiryDate,
                 ChatBanned, ChatBanExpiryDate, LastIP, LastLogoutDate, LastLoginDate,
                 Deleted, DeleteDate,
                 Married, MarriedDate, Mentor, MentorDate, IsMentor, MentorExp,
                 CurrentMapIndex, CurrentLocationX, CurrentLocationY, Direction, BindMapIndex, BindLocationX, BindLocationY,
                 HP, MP, Experience, Gold,
                 AMode, PMode, AllowGroup, AllowTrade, AllowObserve, PKPoints, NewDay,
                 Thrusting, HalfMoon, CrossHalfMoon, DoubleSlash, MentalState, MentalStateLvl,
                 PearlCount, CollectTime)
                VALUES
                (@Id, @AccountId, @Name, @Level, @Class, @Gender, @Hair, @GuildIndex,
                 @CreationIP, @CreationDate, @Banned, @BanReason, @ExpiryDate,
                 @ChatBanned, @ChatBanExpiryDate, @LastIP, @LastLogoutDate, @LastLoginDate,
                 @Deleted, @DeleteDate,
                 @Married, @MarriedDate, @Mentor, @MentorDate, @IsMentor, @MentorExp,
                 @CurrentMapIndex, @CurrentLocationX, @CurrentLocationY, @Direction, @BindMapIndex, @BindLocationX, @BindLocationY,
                 @HP, @MP, @Experience, @Gold,
                 @AMode, @PMode, @AllowGroup, @AllowTrade, @AllowObserve, @PKPoints, @NewDay,
                 @Thrusting, @HalfMoon, @CrossHalfMoon, @DoubleSlash, @MentalState, @MentalStateLvl,
                 @PearlCount, @CollectTime)
                ON DUPLICATE KEY UPDATE
                    AccountId=VALUES(AccountId),
                    Name=VALUES(Name), Level=VALUES(Level), Class=VALUES(Class), Gender=VALUES(Gender), Hair=VALUES(Hair), GuildIndex=VALUES(GuildIndex),
                    CreationIP=VALUES(CreationIP), CreationDate=VALUES(CreationDate), Banned=VALUES(Banned), BanReason=VALUES(BanReason), ExpiryDate=VALUES(ExpiryDate),
                    ChatBanned=VALUES(ChatBanned), ChatBanExpiryDate=VALUES(ChatBanExpiryDate), LastIP=VALUES(LastIP), LastLogoutDate=VALUES(LastLogoutDate), LastLoginDate=VALUES(LastLoginDate),
                    Deleted=VALUES(Deleted), DeleteDate=VALUES(DeleteDate),
                    Married=VALUES(Married), MarriedDate=VALUES(MarriedDate), Mentor=VALUES(Mentor), MentorDate=VALUES(MentorDate), IsMentor=VALUES(IsMentor), MentorExp=VALUES(MentorExp),
                    CurrentMapIndex=VALUES(CurrentMapIndex), CurrentLocationX=VALUES(CurrentLocationX), CurrentLocationY=VALUES(CurrentLocationY), Direction=VALUES(Direction), BindMapIndex=VALUES(BindMapIndex), BindLocationX=VALUES(BindLocationX), BindLocationY=VALUES(BindLocationY),
                    HP=VALUES(HP), MP=VALUES(MP), Experience=VALUES(Experience), Gold=VALUES(Gold),
                    AMode=VALUES(AMode), PMode=VALUES(PMode), AllowGroup=VALUES(AllowGroup), AllowTrade=VALUES(AllowTrade), AllowObserve=VALUES(AllowObserve), PKPoints=VALUES(PKPoints), NewDay=VALUES(NewDay),
                    Thrusting=VALUES(Thrusting), HalfMoon=VALUES(HalfMoon), CrossHalfMoon=VALUES(CrossHalfMoon), DoubleSlash=VALUES(DoubleSlash), MentalState=VALUES(MentalState), MentalStateLvl=VALUES(MentalStateLvl),
                    PearlCount=VALUES(PearlCount), CollectTime=VALUES(CollectTime);";

            // Pre-create parameters
            cmd.Parameters.Add("@Id", MySqlDbType.Int32);
            cmd.Parameters.Add("@AccountId", MySqlDbType.Int32);
            cmd.Parameters.Add("@Name", MySqlDbType.VarChar);
            cmd.Parameters.Add("@Level", MySqlDbType.UInt16);
            cmd.Parameters.Add("@Class", MySqlDbType.UByte);
            cmd.Parameters.Add("@Gender", MySqlDbType.UByte);
            cmd.Parameters.Add("@Hair", MySqlDbType.UByte);
            cmd.Parameters.Add("@GuildIndex", MySqlDbType.Int32);
            cmd.Parameters.Add("@CreationIP", MySqlDbType.VarChar);
            cmd.Parameters.Add("@CreationDate", MySqlDbType.DateTime);
            cmd.Parameters.Add("@Banned", MySqlDbType.Bool);
            cmd.Parameters.Add("@BanReason", MySqlDbType.VarChar);
            cmd.Parameters.Add("@ExpiryDate", MySqlDbType.DateTime);
            cmd.Parameters.Add("@ChatBanned", MySqlDbType.Bool);
            cmd.Parameters.Add("@ChatBanExpiryDate", MySqlDbType.DateTime);
            cmd.Parameters.Add("@LastIP", MySqlDbType.VarChar);
            cmd.Parameters.Add("@LastLogoutDate", MySqlDbType.DateTime);
            cmd.Parameters.Add("@LastLoginDate", MySqlDbType.DateTime);
            cmd.Parameters.Add("@Deleted", MySqlDbType.Bool);
            cmd.Parameters.Add("@DeleteDate", MySqlDbType.DateTime);
            cmd.Parameters.Add("@Married", MySqlDbType.Int32);
            cmd.Parameters.Add("@MarriedDate", MySqlDbType.DateTime);
            cmd.Parameters.Add("@Mentor", MySqlDbType.Int32);
            cmd.Parameters.Add("@MentorDate", MySqlDbType.DateTime);
            cmd.Parameters.Add("@IsMentor", MySqlDbType.Bool);
            cmd.Parameters.Add("@MentorExp", MySqlDbType.Int64);
            cmd.Parameters.Add("@CurrentMapIndex", MySqlDbType.Int32);
            cmd.Parameters.Add("@CurrentLocationX", MySqlDbType.Int32);
            cmd.Parameters.Add("@CurrentLocationY", MySqlDbType.Int32);
            cmd.Parameters.Add("@Direction", MySqlDbType.UByte);
            cmd.Parameters.Add("@BindMapIndex", MySqlDbType.Int32);
            cmd.Parameters.Add("@BindLocationX", MySqlDbType.Int32);
            cmd.Parameters.Add("@BindLocationY", MySqlDbType.Int32);
            cmd.Parameters.Add("@HP", MySqlDbType.Int32);
            cmd.Parameters.Add("@MP", MySqlDbType.Int32);
            cmd.Parameters.Add("@Experience", MySqlDbType.Int64);
            cmd.Parameters.Add("@Gold", MySqlDbType.UInt32);
            cmd.Parameters.Add("@AMode", MySqlDbType.UByte);
            cmd.Parameters.Add("@PMode", MySqlDbType.UByte);
            cmd.Parameters.Add("@AllowGroup", MySqlDbType.Bool);
            cmd.Parameters.Add("@AllowTrade", MySqlDbType.Bool);
            cmd.Parameters.Add("@AllowObserve", MySqlDbType.Bool);
            cmd.Parameters.Add("@PKPoints", MySqlDbType.Int32);
            cmd.Parameters.Add("@NewDay", MySqlDbType.Bool);
            cmd.Parameters.Add("@Thrusting", MySqlDbType.Bool);
            cmd.Parameters.Add("@HalfMoon", MySqlDbType.Bool);
            cmd.Parameters.Add("@CrossHalfMoon", MySqlDbType.Bool);
            cmd.Parameters.Add("@DoubleSlash", MySqlDbType.Bool);
            cmd.Parameters.Add("@MentalState", MySqlDbType.UByte);
            cmd.Parameters.Add("@MentalStateLvl", MySqlDbType.UByte);
            cmd.Parameters.Add("@PearlCount", MySqlDbType.Int32);
            cmd.Parameters.Add("@CollectTime", MySqlDbType.Int64);

            foreach (var (accountId, ch) in characters)
            {
                BindParams(cmd, accountId, ch, includeId:true);
                cmd.ExecuteNonQuery();
            }

            tx.Commit();
        }

        private static void BindParams(MySqlCommand cmd, int accountId, CharacterInfo ch, bool includeId)
        {
            // Ensure parameters exist
            if (includeId && !cmd.Parameters.Contains("@Id")) cmd.Parameters.Add("@Id", MySqlDbType.Int32);
            if (!cmd.Parameters.Contains("@AccountId")) cmd.Parameters.Add("@AccountId", MySqlDbType.Int32);
            if (!cmd.Parameters.Contains("@Name")) cmd.Parameters.Add("@Name", MySqlDbType.VarChar);
            if (!cmd.Parameters.Contains("@Level")) cmd.Parameters.Add("@Level", MySqlDbType.UInt16);
            if (!cmd.Parameters.Contains("@Class")) cmd.Parameters.Add("@Class", MySqlDbType.UByte);
            if (!cmd.Parameters.Contains("@Gender")) cmd.Parameters.Add("@Gender", MySqlDbType.UByte);
            if (!cmd.Parameters.Contains("@Hair")) cmd.Parameters.Add("@Hair", MySqlDbType.UByte);
            if (!cmd.Parameters.Contains("@GuildIndex")) cmd.Parameters.Add("@GuildIndex", MySqlDbType.Int32);
            if (!cmd.Parameters.Contains("@CreationIP")) cmd.Parameters.Add("@CreationIP", MySqlDbType.VarChar);
            if (!cmd.Parameters.Contains("@CreationDate")) cmd.Parameters.Add("@CreationDate", MySqlDbType.DateTime);
            if (!cmd.Parameters.Contains("@Banned")) cmd.Parameters.Add("@Banned", MySqlDbType.Bool);
            if (!cmd.Parameters.Contains("@BanReason")) cmd.Parameters.Add("@BanReason", MySqlDbType.VarChar);
            if (!cmd.Parameters.Contains("@ExpiryDate")) cmd.Parameters.Add("@ExpiryDate", MySqlDbType.DateTime);
            if (!cmd.Parameters.Contains("@ChatBanned")) cmd.Parameters.Add("@ChatBanned", MySqlDbType.Bool);
            if (!cmd.Parameters.Contains("@ChatBanExpiryDate")) cmd.Parameters.Add("@ChatBanExpiryDate", MySqlDbType.DateTime);
            if (!cmd.Parameters.Contains("@LastIP")) cmd.Parameters.Add("@LastIP", MySqlDbType.VarChar);
            if (!cmd.Parameters.Contains("@LastLogoutDate")) cmd.Parameters.Add("@LastLogoutDate", MySqlDbType.DateTime);
            if (!cmd.Parameters.Contains("@LastLoginDate")) cmd.Parameters.Add("@LastLoginDate", MySqlDbType.DateTime);
            if (!cmd.Parameters.Contains("@Deleted")) cmd.Parameters.Add("@Deleted", MySqlDbType.Bool);
            if (!cmd.Parameters.Contains("@DeleteDate")) cmd.Parameters.Add("@DeleteDate", MySqlDbType.DateTime);
            if (!cmd.Parameters.Contains("@Married")) cmd.Parameters.Add("@Married", MySqlDbType.Int32);
            if (!cmd.Parameters.Contains("@MarriedDate")) cmd.Parameters.Add("@MarriedDate", MySqlDbType.DateTime);
            if (!cmd.Parameters.Contains("@Mentor")) cmd.Parameters.Add("@Mentor", MySqlDbType.Int32);
            if (!cmd.Parameters.Contains("@MentorDate")) cmd.Parameters.Add("@MentorDate", MySqlDbType.DateTime);
            if (!cmd.Parameters.Contains("@IsMentor")) cmd.Parameters.Add("@IsMentor", MySqlDbType.Bool);
            if (!cmd.Parameters.Contains("@MentorExp")) cmd.Parameters.Add("@MentorExp", MySqlDbType.Int64);
            if (!cmd.Parameters.Contains("@CurrentMapIndex")) cmd.Parameters.Add("@CurrentMapIndex", MySqlDbType.Int32);
            if (!cmd.Parameters.Contains("@CurrentLocationX")) cmd.Parameters.Add("@CurrentLocationX", MySqlDbType.Int32);
            if (!cmd.Parameters.Contains("@CurrentLocationY")) cmd.Parameters.Add("@CurrentLocationY", MySqlDbType.Int32);
            if (!cmd.Parameters.Contains("@Direction")) cmd.Parameters.Add("@Direction", MySqlDbType.UByte);
            if (!cmd.Parameters.Contains("@BindMapIndex")) cmd.Parameters.Add("@BindMapIndex", MySqlDbType.Int32);
            if (!cmd.Parameters.Contains("@BindLocationX")) cmd.Parameters.Add("@BindLocationX", MySqlDbType.Int32);
            if (!cmd.Parameters.Contains("@BindLocationY")) cmd.Parameters.Add("@BindLocationY", MySqlDbType.Int32);
            if (!cmd.Parameters.Contains("@HP")) cmd.Parameters.Add("@HP", MySqlDbType.Int32);
            if (!cmd.Parameters.Contains("@MP")) cmd.Parameters.Add("@MP", MySqlDbType.Int32);
            if (!cmd.Parameters.Contains("@Experience")) cmd.Parameters.Add("@Experience", MySqlDbType.Int64);
            if (!cmd.Parameters.Contains("@Gold")) cmd.Parameters.Add("@Gold", MySqlDbType.UInt32);
            if (!cmd.Parameters.Contains("@AMode")) cmd.Parameters.Add("@AMode", MySqlDbType.UByte);
            if (!cmd.Parameters.Contains("@PMode")) cmd.Parameters.Add("@PMode", MySqlDbType.UByte);
            if (!cmd.Parameters.Contains("@AllowGroup")) cmd.Parameters.Add("@AllowGroup", MySqlDbType.Bool);
            if (!cmd.Parameters.Contains("@AllowTrade")) cmd.Parameters.Add("@AllowTrade", MySqlDbType.Bool);
            if (!cmd.Parameters.Contains("@AllowObserve")) cmd.Parameters.Add("@AllowObserve", MySqlDbType.Bool);
            if (!cmd.Parameters.Contains("@PKPoints")) cmd.Parameters.Add("@PKPoints", MySqlDbType.Int32);
            if (!cmd.Parameters.Contains("@NewDay")) cmd.Parameters.Add("@NewDay", MySqlDbType.Bool);
            if (!cmd.Parameters.Contains("@Thrusting")) cmd.Parameters.Add("@Thrusting", MySqlDbType.Bool);
            if (!cmd.Parameters.Contains("@HalfMoon")) cmd.Parameters.Add("@HalfMoon", MySqlDbType.Bool);
            if (!cmd.Parameters.Contains("@CrossHalfMoon")) cmd.Parameters.Add("@CrossHalfMoon", MySqlDbType.Bool);
            if (!cmd.Parameters.Contains("@DoubleSlash")) cmd.Parameters.Add("@DoubleSlash", MySqlDbType.Bool);
            if (!cmd.Parameters.Contains("@MentalState")) cmd.Parameters.Add("@MentalState", MySqlDbType.UByte);
            if (!cmd.Parameters.Contains("@MentalStateLvl")) cmd.Parameters.Add("@MentalStateLvl", MySqlDbType.UByte);
            if (!cmd.Parameters.Contains("@PearlCount")) cmd.Parameters.Add("@PearlCount", MySqlDbType.Int32);
            if (!cmd.Parameters.Contains("@CollectTime")) cmd.Parameters.Add("@CollectTime", MySqlDbType.Int64);

            if (includeId) cmd.Parameters["@Id"].Value = ch.Index;
            cmd.Parameters["@AccountId"].Value = accountId;
            cmd.Parameters["@Name"].Value = ch.Name ?? string.Empty;
            cmd.Parameters["@Level"].Value = ch.Level;
            cmd.Parameters["@Class"].Value = (byte)ch.Class;
            cmd.Parameters["@Gender"].Value = (byte)ch.Gender;
            cmd.Parameters["@Hair"].Value = ch.Hair;
            cmd.Parameters["@GuildIndex"].Value = ch.GuildIndex;
            cmd.Parameters["@CreationIP"].Value = ch.CreationIP ?? string.Empty;
            cmd.Parameters["@CreationDate"].Value = ch.CreationDate == DateTime.MinValue ? DateTime.UtcNow : ch.CreationDate;
            cmd.Parameters["@Banned"].Value = ch.Banned;
            cmd.Parameters["@BanReason"].Value = ch.BanReason ?? string.Empty;
            cmd.Parameters["@ExpiryDate"].Value = ch.ExpiryDate == DateTime.MinValue ? (object)DBNull.Value : ch.ExpiryDate;
            cmd.Parameters["@ChatBanned"].Value = ch.ChatBanned;
            cmd.Parameters["@ChatBanExpiryDate"].Value = ch.ChatBanExpiryDate == DateTime.MinValue ? (object)DBNull.Value : ch.ChatBanExpiryDate;
            cmd.Parameters["@LastIP"].Value = ch.LastIP ?? string.Empty;
            cmd.Parameters["@LastLogoutDate"].Value = ch.LastLogoutDate == DateTime.MinValue ? (object)DBNull.Value : ch.LastLogoutDate;
            cmd.Parameters["@LastLoginDate"].Value = ch.LastLoginDate == DateTime.MinValue ? (object)DBNull.Value : ch.LastLoginDate;
            cmd.Parameters["@Deleted"].Value = ch.Deleted;
            cmd.Parameters["@DeleteDate"].Value = ch.DeleteDate == DateTime.MinValue ? (object)DBNull.Value : ch.DeleteDate;
            cmd.Parameters["@Married"].Value = ch.Married;
            cmd.Parameters["@MarriedDate"].Value = ch.MarriedDate == DateTime.MinValue ? (object)DBNull.Value : ch.MarriedDate;
            cmd.Parameters["@Mentor"].Value = ch.Mentor;
            cmd.Parameters["@MentorDate"].Value = ch.MentorDate == DateTime.MinValue ? (object)DBNull.Value : ch.MentorDate;
            cmd.Parameters["@IsMentor"].Value = ch.IsMentor;
            cmd.Parameters["@MentorExp"].Value = ch.MentorExp;
            cmd.Parameters["@CurrentMapIndex"].Value = ch.CurrentMapIndex;
            cmd.Parameters["@CurrentLocationX"].Value = ch.CurrentLocation.X;
            cmd.Parameters["@CurrentLocationY"].Value = ch.CurrentLocation.Y;
            cmd.Parameters["@Direction"].Value = (byte)ch.Direction;
            cmd.Parameters["@BindMapIndex"].Value = ch.BindMapIndex;
            cmd.Parameters["@BindLocationX"].Value = ch.BindLocation.X;
            cmd.Parameters["@BindLocationY"].Value = ch.BindLocation.Y;
            cmd.Parameters["@HP"].Value = ch.HP;
            cmd.Parameters["@MP"].Value = ch.MP;
            cmd.Parameters["@Experience"].Value = ch.Experience;
            cmd.Parameters["@Gold"].Value = ch.Gold;
            cmd.Parameters["@AMode"].Value = (byte)ch.AMode;
            cmd.Parameters["@PMode"].Value = (byte)ch.PMode;
            cmd.Parameters["@AllowGroup"].Value = ch.AllowGroup;
            cmd.Parameters["@AllowTrade"].Value = ch.AllowTrade;
            cmd.Parameters["@AllowObserve"].Value = ch.AllowObserve;
            cmd.Parameters["@PKPoints"].Value = ch.PKPoints;
            cmd.Parameters["@NewDay"].Value = ch.NewDay;
            cmd.Parameters["@Thrusting"].Value = ch.Thrusting;
            cmd.Parameters["@HalfMoon"].Value = ch.HalfMoon;
            cmd.Parameters["@CrossHalfMoon"].Value = ch.CrossHalfMoon;
            cmd.Parameters["@DoubleSlash"].Value = ch.DoubleSlash;
            cmd.Parameters["@MentalState"].Value = ch.MentalState;
            cmd.Parameters["@MentalStateLvl"].Value = ch.MentalStateLvl;
            cmd.Parameters["@PearlCount"].Value = ch.PearlCount;
            cmd.Parameters["@CollectTime"].Value = ch.CollectTime;
        }

        private static CharacterInfo MapCharacter(MySqlDataReader reader)
        {
            var ch = new CharacterInfo
            {
                Index = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Level = reader.GetFieldValue<ushort>(reader.GetOrdinal("Level")),
                Class = (MirClass)reader.GetByte(reader.GetOrdinal("Class")),
                Gender = (MirGender)reader.GetByte(reader.GetOrdinal("Gender")),
                Hair = reader.GetByte(reader.GetOrdinal("Hair")),
                GuildIndex = reader.GetInt32(reader.GetOrdinal("GuildIndex")),
                CreationIP = reader.GetString(reader.GetOrdinal("CreationIP")),
                CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                Banned = reader.GetBoolean(reader.GetOrdinal("Banned")),
                BanReason = reader.IsDBNull(reader.GetOrdinal("BanReason")) ? string.Empty : reader.GetString(reader.GetOrdinal("BanReason")),
                ChatBanned = reader.GetBoolean(reader.GetOrdinal("ChatBanned")),
                LastIP = reader.IsDBNull(reader.GetOrdinal("LastIP")) ? string.Empty : reader.GetString(reader.GetOrdinal("LastIP")),
                Deleted = reader.GetBoolean(reader.GetOrdinal("Deleted")),
                Married = reader.GetInt32(reader.GetOrdinal("Married")),
                Mentor = reader.GetInt32(reader.GetOrdinal("Mentor")),
                IsMentor = reader.GetBoolean(reader.GetOrdinal("IsMentor")),
                MentorExp = reader.GetInt64(reader.GetOrdinal("MentorExp")),
                CurrentMapIndex = reader.GetInt32(reader.GetOrdinal("CurrentMapIndex")),
                Direction = (MirDirection)reader.GetByte(reader.GetOrdinal("Direction")),
                BindMapIndex = reader.GetInt32(reader.GetOrdinal("BindMapIndex")),
                HP = reader.GetInt32(reader.GetOrdinal("HP")),
                MP = reader.GetInt32(reader.GetOrdinal("MP")),
                Experience = reader.GetInt64(reader.GetOrdinal("Experience")),
                Gold = reader.GetFieldValue<uint>(reader.GetOrdinal("Gold")),
                AMode = (AttackMode)reader.GetByte(reader.GetOrdinal("AMode")),
                PMode = (PetMode)reader.GetByte(reader.GetOrdinal("PMode")),
                AllowGroup = reader.GetBoolean(reader.GetOrdinal("AllowGroup")),
                AllowTrade = reader.GetBoolean(reader.GetOrdinal("AllowTrade")),
                AllowObserve = reader.GetBoolean(reader.GetOrdinal("AllowObserve")),
                PKPoints = reader.GetInt32(reader.GetOrdinal("PKPoints")),
                NewDay = reader.GetBoolean(reader.GetOrdinal("NewDay")),
                Thrusting = reader.GetBoolean(reader.GetOrdinal("Thrusting")),
                HalfMoon = reader.GetBoolean(reader.GetOrdinal("HalfMoon")),
                CrossHalfMoon = reader.GetBoolean(reader.GetOrdinal("CrossHalfMoon")),
                DoubleSlash = reader.GetBoolean(reader.GetOrdinal("DoubleSlash")),
                MentalState = reader.GetByte(reader.GetOrdinal("MentalState")),
                MentalStateLvl = reader.GetByte(reader.GetOrdinal("MentalStateLvl")),
                PearlCount = reader.GetInt32(reader.GetOrdinal("PearlCount"))
            };

            // Nullable DateTimes
            ch.ExpiryDate = reader.IsDBNull(reader.GetOrdinal("ExpiryDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("ExpiryDate"));
            ch.ChatBanExpiryDate = reader.IsDBNull(reader.GetOrdinal("ChatBanExpiryDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("ChatBanExpiryDate"));
            ch.LastLogoutDate = reader.IsDBNull(reader.GetOrdinal("LastLogoutDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("LastLogoutDate"));
            ch.LastLoginDate = reader.IsDBNull(reader.GetOrdinal("LastLoginDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("LastLoginDate"));
            ch.DeleteDate = reader.IsDBNull(reader.GetOrdinal("DeleteDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("DeleteDate"));
            ch.MarriedDate = reader.IsDBNull(reader.GetOrdinal("MarriedDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("MarriedDate"));
            ch.MentorDate = reader.IsDBNull(reader.GetOrdinal("MentorDate")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("MentorDate"));

            // Points
            var cx = reader.GetInt32(reader.GetOrdinal("CurrentLocationX"));
            var cy = reader.GetInt32(reader.GetOrdinal("CurrentLocationY"));
            ch.CurrentLocation = new System.Drawing.Point(cx, cy);
            var bx = reader.GetInt32(reader.GetOrdinal("BindLocationX"));
            var by = reader.GetInt32(reader.GetOrdinal("BindLocationY"));
            ch.BindLocation = new System.Drawing.Point(bx, by);

            // Misc
            ch.CollectTime = reader.GetInt64(reader.GetOrdinal("CollectTime"));

            return ch;
        }
    }
}
