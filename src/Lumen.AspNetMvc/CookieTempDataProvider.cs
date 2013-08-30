/* New BSD License
 * ---------------
 * Copyright ©2012, Brock Allen. All Rights Reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the 
 * following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following 
 *    disclaimer.
 * 
 * 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following
 *    disclaimer in the documentation and/or other materials provided with the distribution.
 * 
 * 3. The name of the author may not be used to endorse or promote products derived from this software without specific
 *    prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY Brock Allen "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * */

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Lumen.AspNetMvc
{

    /// <summary>
    /// A cookie temp data provider.
    /// </summary>
    /// <remarks>
    /// Adapted from https://github.com/brockallen/CookieTempData
    /// </remarks>
    public class CookieTempDataProvider : ITempDataProvider
    {
        private const string CookieName = "flash";

        /// <inheritdoc />
        public IDictionary<string, object> LoadTempData(ControllerContext controllerContext)
        {
            var value = GetCookieValue(controllerContext);
            return GetDictionaryFromCookieValue(value);
        }

        private static string GetCookieValue(ControllerContext controllerContext)
        {
            if (controllerContext.HttpContext.Request.Cookies.AllKeys.Contains(CookieName))
            {
                var cookie = controllerContext.HttpContext.Request.Cookies[CookieName];
                if (cookie != null)
                {
                    return cookie.Value;
                }
            }

            return null;
        }

        private static IDictionary<string, object> GetDictionaryFromCookieValue(string value)
        {
            var bytes = Unprotect(value);
            bytes = Decompress(bytes);

            return DeserializeWithBinaryFormatter(bytes);
        }

        private static byte[] Unprotect(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return MachineKey.Decode(value, MachineKeyProtection.All);
        }

        private static byte[] Decompress(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            using (var input = new MemoryStream(data))
            using (var output = new MemoryStream())
            {
                using (var stream = new DeflateStream(input, CompressionMode.Decompress))
                {
                    stream.CopyTo(output);
                }

                return output.ToArray();
            }
        }

        private static IDictionary<string, object> DeserializeWithBinaryFormatter(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            using (var ms = new MemoryStream(data))
            {
                var formatter = new BinaryFormatter();
                var obj = formatter.Deserialize(ms);

                return obj as IDictionary<string, object>;
            }
        }

        /// <inheritdoc />
        public void SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values)
        {
            var value = GetCookieValueFromDictionary(values);
            IssueCookie(controllerContext, value);
        }

        private static string GetCookieValueFromDictionary(IDictionary<string, object> values)
        {
            byte[] bytes = SerializeWithBinaryFormatter(values);
            bytes = Compress(bytes);

            return Protect(bytes);
        }

        private static byte[] Compress(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            using (var input = new MemoryStream(data))
            using (var output = new MemoryStream())
            {
                using (var stream = new DeflateStream(output, CompressionMode.Compress))
                {
                    input.CopyTo(stream);
                }

                return output.ToArray();
            }
        }

        private static string Protect(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }

            return MachineKey.Encode(data, MachineKeyProtection.All);
        }

        private static void IssueCookie(ControllerContext controllerContext, string value)
        {
            // if we don't have a value and there's no prior cookie then exit
            if (value == null && !controllerContext.HttpContext.Request.Cookies.AllKeys.Contains(CookieName))
            {
                return;
            }

            var cookie = new HttpCookie(CookieName, value)
            {
                // don't allow javascript access to the cookie
                HttpOnly = true,
                // set the path so other apps on the same server don't see the cookie
                Path = controllerContext.HttpContext.Request.ApplicationPath,
                // ideally we're always going over SSL, but be flexible for non-SSL apps
                Secure = controllerContext.HttpContext.Request.IsSecureConnection
            };

            if (value == null)
            {
                // if we have no data then issue an expired cookie to clear the cookie
                cookie.Expires = DateTime.Now.AddMonths(-1);
            }

            controllerContext.HttpContext.Response.Cookies.Add(cookie);
        }

        private static byte[] SerializeWithBinaryFormatter(IDictionary<string, object> data)
        {
            if (data == null || data.Keys.Count == 0)
            {
                return null;
            }

            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, data);

                ms.Seek(0, SeekOrigin.Begin);

                return ms.ToArray();
            }
        }
    }
}