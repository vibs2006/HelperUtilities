using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace HelperUtilties.ConfigManager
{
    public static class ConfigMethods
    {
        public static string getConnectionString(string keyName)
        {
            try
            {
                return ConfigurationManager.ConnectionStrings[keyName].ConnectionString;
            }
            catch(Exception ex)
            {
                throw new Exception($"Error in Returning Connection String from Config File. Is connection {keyName} present in config file?", ex);
            }            
        }

        public static string getAppSettingString(string keyName)
        {
            try
            {
                return ConfigurationManager.AppSettings.Get(keyName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in Returning Appsettings String from Config File. Is connection {keyName} present in config file?", ex);
            }
        }

        public static string generateConnectionKeyName(string brandName, string defaultConcatString = "connect")
        {
            brandName = brandName.Trim().ToLower();
            if (brandName.Contains("ph") || brandName.Contains("parcelhero"))
            {
                return "parcelhero" + defaultConcatString;
            }
            else if (brandName.Contains("dp") || brandName.Contains("deliverplus"))
            {
                return "deliverplus" + defaultConcatString;
            }
            else if (brandName.Contains("fl") || brandName.Contains("fastlane") || brandName.Contains("pc") || brandName.Contains("parcelcompare"))
            {
                return "fastlane" + defaultConcatString;
            }
            else if (brandName.Contains("pa") || brandName.Contains("parceladmin"))
            {
                return "parceladmin" + defaultConcatString;
            }
            else
            {
                throw new Exception("Invalid Brandname specified in function `generateConnectionKeyName(string brandName....)`");
            }
        }

        public static void AddOrModifyAppSettings(string key, string value)
        {

            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            if (settings[key] == null)
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
    }
}
