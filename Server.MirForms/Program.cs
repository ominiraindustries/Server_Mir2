using System;
using System.IO;

namespace Server.MirForms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Packet.IsServer = true;

            // Ensure log directory exists and load log4net from local config if present (fallback: default config)
            try
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                Directory.CreateDirectory(Path.Combine(baseDir, "Logs"));
                var cfgPath = Path.Combine(baseDir, "log4net.config");
                var fi = new FileInfo(cfgPath);
                if (fi.Exists)
                    log4net.Config.XmlConfigurator.Configure(fi);
                else
                    log4net.Config.XmlConfigurator.Configure();
            }
            catch { /* ignore logging config errors */ }

            try
            {
                Settings.Load();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SMain());

                Settings.Save();
            }
            catch(Exception ex)
            {
                Logger.GetLogger().Error(ex);
            }
        }
    }
}
