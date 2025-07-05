using System.Security.Cryptography;
using System.Text;

public class AesEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public AesEncryptionService(IConfiguration config)
    {

        var keyString = Environment.GetEnvironmentVariable("Encryption__Key"); //these are the values that we take from the environment variables
        var ivString =  Environment.GetEnvironmentVariable("Encryption__IV");

        if (string.IsNullOrWhiteSpace(keyString) || string.IsNullOrWhiteSpace(ivString))
        {
            throw new Exception("Encryption key or IV not configured properly.");
        }
        _key = Convert.FromBase64String(keyString);
        _iv = Convert.FromBase64String(ivString);
    }

    public string Encrypt(string plaintext)
    {
        using Aes aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);

        sw.Write(plaintext);
        sw.Close();

        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string ciphertext)
    {
        using Aes aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(Convert.FromBase64String(ciphertext));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}
