using System;
using System.Configuration;
using System.Web.Security;

namespace Lumen.AspNetMvc.Security.AppHarbor
{
    public class ConfigFileAuthenticationConfiguration
    {
        public string CookieName
        {
            get { return "coderesque.auth"; }
        }

        public string EncryptionAlgorithm
        {
            get { return "rijndael"; }
        }

        public byte[] EncryptionKey
        {
            get { return GetRequiredSetting("cookieauthentication.encryptionkey").GetByteArrayFromHexString(); }
        }

        public string LoginUrl
        {
            get { return FormsAuthentication.LoginUrl; }
        }

        public bool RequireSSL
        {
            get { return FormsAuthentication.RequireSSL; }
        }

        public bool SlidingExpiration
        {
            get { return FormsAuthentication.SlidingExpiration; }
        }

        public TimeSpan Timeout
        {
            get { return FormsAuthentication.Timeout; }
        }

        public string ValidationAlgorithm
        {
            get { return "hmacsha256"; }
        }

        public byte[] ValidationKey
        {
            get { return GetRequiredSetting("cookieauthentication.validationkey").GetByteArrayFromHexString(); }
        }

        private static string GetRequiredSetting(string name)
        {
            var setting = ConfigurationManager.AppSettings[name];
            if (!string.IsNullOrWhiteSpace(setting))
            {
                return setting;
            }

            string message = string.Format("Required setting '{0}' not found or null/empty.", name);
            throw new InvalidOperationException(message);
        }
    }
}
