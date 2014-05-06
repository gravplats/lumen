using System;
using System.Text;

namespace Lumen.AspNetMvc.Security.AppHarbor
{
    public class CookieProtector : IDisposable
    {
        private readonly Encryption encryption;
        private readonly Validation validation;

        public CookieProtector(ConfigFileAuthenticationConfiguration configuration)
        {
            encryption = Encryption.Create(configuration.EncryptionAlgorithm, configuration.EncryptionKey);
            validation = Validation.Create(configuration.ValidationAlgorithm, configuration.ValidationKey);
        }

        public void Dispose()
        {
            encryption.Dispose();
            validation.Dispose();
        }

        public string Protect(string data)
        {
            return Protect(Encoding.UTF8.GetBytes(data));
        }

        public string Protect(byte[] data)
        {
            data = encryption.Encrypt(data);
            data = validation.Sign(data);

            var versionedData = new byte[data.Length + 1];
            Buffer.BlockCopy(data, 0, versionedData, 1, data.Length);

            return Encode(versionedData);
        }

        public bool Validate(string cookie, out string data)
        {
            byte[] cookieData;
            if (Validate(cookie, out cookieData))
            {
                data = Encoding.UTF8.GetString(cookieData);
                return true;
            }

            data = null;

            return false;
        }

        public bool Validate(string cookie, out byte[] data)
        {
            data = null;
            try
            {
                var versionedCookieData = Decode(cookie);

                if (versionedCookieData.Length == 0 || versionedCookieData[0] != 0)
                {
                    return false;
                }

                var cookieData = new byte[versionedCookieData.Length - 1];
                Buffer.BlockCopy(versionedCookieData, 1, cookieData, 0, cookieData.Length);

                if (!validation.Validate(cookieData))
                {
                    return false;
                }

                cookieData = validation.StripSignature(cookieData);
                cookieData = encryption.Decrypt(cookieData);

                data = cookieData;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string Encode(byte[] data)
        {
            // OWIN assumes that the data is Base64 URL encoded.
            return Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static byte[] Decode(string text)
        {
            // OWIN assumes that the data is Base64 URL encoded.
            return Convert.FromBase64String(Pad(text.Replace('-', '+').Replace('_', '/')));
        }

        private static string Pad(string text)
        {
            var padding = 3 - ((text.Length + 3) % 4);
            if (padding == 0)
            {
                return text;
            }

            return text + new string('=', padding);
        }
    }
}
