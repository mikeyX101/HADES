using System;
using System.Security.Cryptography;
using System.Text;

namespace HADES.Util
{
	public class EncryptionUtil
    {

        private static byte[] key = new byte[] { 64, 5, 221, 17, 116, 101, 241, 129 };
        private static byte[] iv = new byte[] { 100, 154, 137, 233, 200, 166, 106, 66 };

        // Encrypt password
        public static string Encrypt(string pass)
        {
            if (pass == null) return null;

            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
            byte[] inputbuffer = Encoding.Unicode.GetBytes(pass);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        // Decrypt password
        public static string Decrypt(string pass)
        {
            if (pass == null) return null;

            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
            byte[] inputbuffer = Convert.FromBase64String(pass);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.Unicode.GetString(outputBuffer);
        }
    }


}
