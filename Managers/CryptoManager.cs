using System.Security.Cryptography;
using System.Text;

namespace PraticaSockets
{
    public static class CryptoManager
    {
        private static ECDiffieHellmanCng _diffieHellman = new ECDiffieHellmanCng
        {
            KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
            HashAlgorithm = CngAlgorithm.Sha256
        };
        public static byte[] PublicKey => _diffieHellman.PublicKey.ToByteArray();

        public static byte[] GetSharedSecret(byte[] otherPartyPublicKey)
        {
            return _diffieHellman.DeriveKeyMaterial(CngKey.Import(otherPartyPublicKey, CngKeyBlobFormat.EccPublicBlob));
        }

        public static dynamic Encrypt(string data, byte[] clientPublicKey)
        {
            using Aes aes = Aes.Create();
            aes.Key = GetSharedSecret(clientPublicKey);
            var iv = aes.IV;

            // Encrypt the message
            using MemoryStream ciphertext = new();
            using CryptoStream cs = new(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write);
            byte[] plaintextMessage = Encoding.UTF8.GetBytes(data);
            cs.Write(plaintextMessage, 0, plaintextMessage.Length);
            cs.Close();
            return new { encryptedMessage = ciphertext.ToArray(), IV = iv };
        }

        public static dynamic Decrypt(byte[] data, byte[] iv, byte[] clientPublicKey, bool returnBytes = false)
        {
            using Aes aes = Aes.Create();
            aes.Key = GetSharedSecret(clientPublicKey);
            aes.IV = iv;

            // Decrypt the message
            using MemoryStream plaintext = new();
            using CryptoStream cs = new(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.Close();
            return returnBytes ? plaintext.ToArray() : Encoding.UTF8.GetString(plaintext.ToArray());
        }

        public static byte[] Hash(byte[] data)
        {
            return SHA256.HashData(data);
        }

        public static bool Authenticate(string publicParams, string authMessage, byte[] signature, byte[] hashedAuthMessage)
        {
            using RSACryptoServiceProvider rsa = new();
            rsa.FromXmlString(publicParams);
            var localHashedAuthMessage = Hash(Encoding.UTF8.GetBytes(authMessage));
            return rsa.VerifyData(Encoding.UTF8.GetBytes(authMessage), SHA256.Create(), signature) && hashedAuthMessage.SequenceEqual(localHashedAuthMessage);
        }
        public static bool Authenticate(string publicParams, string authMessage, byte[] signature)
        {
            using RSACryptoServiceProvider rsa = new();
            rsa.FromXmlString(publicParams);
            return rsa.VerifyData(Encoding.UTF8.GetBytes(authMessage), SHA256.Create(), signature);
        }
    }
}