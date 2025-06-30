using DevSpaceWeb.Data;
using System.Security.Cryptography;

namespace DevSpaceWeb;

public class Crypt
{
    public static string EncryptString(string plainText)
    {
        byte[] array;
        using (Aes aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(_Data.Config.EncryptionKey);
            aes.IV = Convert.FromBase64String(_Data.Config.EncryptionIV);
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }

                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);
    }

    public static Tuple<string, string> GenKey()
    {
        using (Aes aesAlgorithm = Aes.Create())
        {
            aesAlgorithm.KeySize = 256;
            aesAlgorithm.GenerateKey();
            aesAlgorithm.GenerateIV();
            string keyBase64 = Convert.ToBase64String(aesAlgorithm.Key);
            string vectorBase64 = Convert.ToBase64String(aesAlgorithm.IV);
            return Tuple.Create(keyBase64, vectorBase64);
        }
    }

    public static string DecryptString(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return null;

        byte[] buffer = Convert.FromBase64String(cipherText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(_Data.Config.EncryptionKey);
            aes.IV = Convert.FromBase64String(_Data.Config.EncryptionIV);
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }
}