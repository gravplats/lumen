using System;
using System.IO;
using System.Security.Cryptography;

namespace Lumen.AspNetMvc.Security.AppHarbor
{
    public class SymmetricEncryption : Encryption
    {
        private readonly SymmetricAlgorithm algorithm;
        private readonly byte[] secretKey;

        public SymmetricEncryption(SymmetricAlgorithm algorithm, byte[] secretKey)
        {
            this.algorithm = algorithm;
            this.secretKey = secretKey;
        }

        public override byte[] Decrypt(byte[] encryptedValue, byte[] initializationVector = null)
        {
            int dataOffset = 0;
            if (initializationVector == null)
            {
                initializationVector = new byte[algorithm.BlockSize / 8];
                Buffer.BlockCopy(encryptedValue, 0, initializationVector, 0, initializationVector.Length);
                dataOffset = initializationVector.Length;
            }

            using (var output = new MemoryStream())
            {
                using (var cryptoOutput = new CryptoStream(output, algorithm.CreateDecryptor(secretKey, initializationVector), CryptoStreamMode.Write))
                {
                    cryptoOutput.Write(encryptedValue, dataOffset, encryptedValue.Length - dataOffset);
                }

                return output.ToArray();
            }
        }

        public override void Dispose()
        {
            algorithm.Dispose();
        }

        public override byte[] Encrypt(byte[] valueBytes, byte[] initializationVector = null)
        {
            bool generateRandomIV = initializationVector == null;
            if (generateRandomIV)
            {
                initializationVector = new byte[algorithm.BlockSize / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(initializationVector);
                }
            }

            using (var output = new MemoryStream())
            {
                if (generateRandomIV)
                {
                    output.Write(initializationVector, 0, initializationVector.Length);
                }

                using (var cryptoOutput = new CryptoStream(output, algorithm.CreateEncryptor(secretKey, initializationVector), CryptoStreamMode.Write))
                {
                    cryptoOutput.Write(valueBytes, 0, valueBytes.Length);
                }

                return output.ToArray();
            }
        }
    }

    public class SymmetricEncryption<T> : SymmetricEncryption where T : SymmetricAlgorithm, new()
    {
        public SymmetricEncryption(byte[] secretKey) : base(new T(), secretKey) { }
    }
}
