using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Security
{
    public class SecurityModelCrypto
    {
        public static string Sault { get; set; }
        public static bool ByPass { get; set; }
        private static RijndaelManaged AES { get; set; }
        private static int AesSaultSize = 2;
        static SecurityModelCrypto()
        {
            Sault = "CSI.Security";
            ByPass = false;
            AES = new RijndaelManaged();
            AES.GenerateKey();
            AES.GenerateIV();
        }
        public static string HashEncrypt(string data)
        {
            if (ByPass)
                return data;

            MD5 hasher = MD5.Create();
            var bits = hasher.ComputeHash(System.Text.Encoding.ASCII.GetBytes("CSI.Security" + data + Sault.ToUpper()));
            return System.Convert.ToBase64String(bits);
        }
        public static string Encrypt(string data)
        {
            Random rand = new Random();
            byte[] saultBytes = new byte[AesSaultSize];
            rand.NextBytes(saultBytes);

            var bytes = System.Text.Encoding.ASCII.GetBytes(data);
            bytes = saultBytes.Concat(bytes).ToArray();

            using (var aes = AES.CreateEncryptor())
                bytes = aes.TransformFinalBlock(bytes, 0, bytes.Length);
            var b64 = System.Convert.ToBase64String(bytes);

            return b64;
        }
        public static string Decrypt(string data)
        {
            try
            {
                var bytes = System.Convert.FromBase64String(data);
                using (var aes = AES.CreateDecryptor())
                    bytes = aes.TransformFinalBlock(bytes, 0, bytes.Length);

                var text = System.Text.Encoding.ASCII.GetString(bytes, AesSaultSize, bytes.Length - AesSaultSize);
                return text;
            }
            catch (FormatException)
            {
                return string.Empty;
            }
            catch (CryptographicException)
            {
                return string.Empty;
            }
        }
    }
}
