using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DesktopExample.AppCode
{
    internal class Settings
    {
        public string apiKey { get; set; }


        public string options { get; set; }
        

        public const string ApiBaseUrl = "https://api.simplylookupadmin.co.uk";
        private const string keyPath = @"Software\SimplyPostcode";

        public bool Save()
        {          
            RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath);

            if (key != null)
            {
                key.SetValue("apiKey", apiKey);
                key.Close();

                return true;
            }
            return false;
        }
        public bool Load()
        {
            apiKey = (string)Registry.GetValue($@"HKEY_CURRENT_USER\{keyPath}", "apiKey", "");

            return (apiKey != "");
        }
    }
}
