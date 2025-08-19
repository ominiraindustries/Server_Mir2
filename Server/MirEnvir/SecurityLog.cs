using System;
using System.IO;

namespace Server.MirEnvir
{
    public static class SecurityLog
    {
        private static readonly object _lock = new object();

        private static string LogFilePath
        {
            get
            {
                var dir = Settings.LogsPath;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var fileName = Settings.SecurityLogToFile ? $"Security_{DateTime.UtcNow:yyyyMMdd}.log" : "Security.log";
                return Path.Combine(dir, fileName);
            }
        }

        public static void Economy(string account, string character, string action, uint amount, uint before, uint after, string meta = null)
        {
            if (!Settings.EnableSecurityLogs) return;
            var line = $"[{DateTime.UtcNow:O}] ECONOMY account={account ?? "?"} char={character ?? "?"} action={action} amount={amount} before={before} after={after} meta={meta ?? string.Empty}";
            Write(line);
        }

        public static void Item(
            string account,
            string character,
            string action,
            ulong uniqueId,
            int itemIndex,
            ushort count,
            string src,
            string dst,
            string map,
            int x,
            int y,
            string meta = null)
        {
            if (!Settings.EnableSecurityLogs) return;
            var line =
                $"[{DateTime.UtcNow:O}] ITEM account={account ?? "?"} char={character ?? "?"} action={action} uid={uniqueId} index={itemIndex} count={count} src={src} dst={dst} map={map ?? "?"} x={x} y={y} meta={meta ?? string.Empty}";
            Write(line);
        }

        public static void Notice(string category, string message)
        {
            if (!Settings.EnableSecurityLogs) return;
            var line = $"[{DateTime.UtcNow:O}] {category.ToUpperInvariant()} {message}";
            Write(line);
        }

        private static void Write(string line)
        {
            try
            {
                lock (_lock)
                {
                    File.AppendAllText(LogFilePath, line + Environment.NewLine);
                }
            }
            catch
            {
                // Avoid throwing inside server loop; logging best-effort only.
            }
        }
    }
}
