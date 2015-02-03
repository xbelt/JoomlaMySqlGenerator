using System;
using System.IO;
using VAkos;

namespace JoomlaMySqlGenerator
{
    static class Config
    {
        public static ConfigSetting Settings;
        public static Xmlconfig Xmlconfig;

        public static void Init()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var userFilePath = Path.Combine(localAppData, "LukasHaefliger");

            if (!Directory.Exists(userFilePath))
                Directory.CreateDirectory(userFilePath);
            var configFilePath = Path.Combine(userFilePath, "mysqlGenConfig.xml");
            var alreadyExists = File.Exists(configFilePath);
            var config = new Xmlconfig(configFilePath, true);
            Xmlconfig = config;
            Settings = config.Settings;
            config.CommitOnUnload = true;
            if (alreadyExists)
                return;
            #region initDefault
            Settings["server"]["MySQL"]["ServerIP"].Value = "";
            Settings["server"]["MySQL"]["User"].Value = "";
            Settings["server"]["MySQL"]["Database"].Value = "";
            Settings["server"]["MySQL"]["Port"].Value = "3306";
            Settings["server"]["MySQL"]["Password"].Value = "";
            Settings["server"]["MySQL"]["Prefix"].Value = "";
            config.Commit();

            #endregion
        }

        public static void Commit()
        {
            Xmlconfig.Commit();
        }
    }
}
