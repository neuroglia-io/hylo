using System.Security.Cryptography;
using System.Text;

namespace Hylo;

/// <summary>
/// Exposes helper methods to generate hashes
/// </summary>
public static class HashHelper
{

    /// <summary>
    /// Gets the default hash key size
    /// </summary>
    public const int KeySize = 64;
    /// <summary>
    /// Gets the default amount of iterations to perform
    /// </summary>
    public const int Iterations = 350000;

    /// <summary>
    /// Hashes the specified value
    /// </summary>
    /// <param name="algorithm">The name of the hash algorithm to use</param>
    /// <param name="data">The data to hash</param>
    /// <param name="salt">The salt to use</param>
    /// <param name="keySize">The size of the key to use</param>
    /// <param name="iterations">The numbers of iterations to run</param>
    /// <returns>The hexadecimal hash</returns>
    public static string Hash(HashAlgorithmName algorithm, byte[] data, byte[] salt, int keySize = KeySize, int iterations = Iterations)
    {
        var hash = Rfc2898DeriveBytes.Pbkdf2(data, salt, iterations, algorithm, keySize);
        return Convert.ToHexString(hash);
    }

    /// <summary>
    /// Hashes the specified value
    /// </summary>
    /// <param name="algorithm">The name of the hash algorithm to use</param>
    /// <param name="data">The data to hash</param>
    /// <param name="salt">The salt that has been used</param>
    /// <param name="keySize">The size of the key to use</param>
    /// <param name="iterations">The numbers of iterations to run</param>
    /// <returns>The hexadecimal hash</returns>
    public static string Hash(HashAlgorithmName algorithm, byte[] data, out byte[] salt, int keySize = KeySize, int iterations = Iterations)
    {
        salt = RandomNumberGenerator.GetBytes(keySize);
        return Hash(algorithm, data, salt, keySize, iterations);

    }

    /// <summary>
    /// Hashes the specified value
    /// </summary>
    /// <param name="algorithm">The name of the hash algorithm to use</param>
    /// <param name="data">The data to hash</param>
    /// <param name="salt">The base64 encoded salt to use</param>
    /// <param name="keySize">The size of the key to use</param>
    /// <param name="iterations">The numbers of iterations to run</param>
    /// <returns>The hexadecimal hash</returns>
    public static string Hash(HashAlgorithmName algorithm, string data, string salt, int keySize = KeySize, int iterations = Iterations)
    {
        return Hash(algorithm, Encoding.UTF8.GetBytes(data), Convert.FromBase64String(salt), keySize, iterations);
    }

    /// <summary>
    /// Hashes the specified value
    /// </summary>
    /// <param name="algorithm">The name of the hash algorithm to use</param>
    /// <param name="data">The data to hash</param>
    /// <param name="salt">The base64 encoded salt to use</param>
    /// <param name="keySize">The size of the key to use</param>
    /// <param name="iterations">The numbers of iterations to run</param>
    /// <returns>The hexadecimal hash</returns>
    public static string Hash(HashAlgorithmName algorithm, string data, out string salt, int keySize = KeySize, int iterations = Iterations)
    {
        var hash = Hash(algorithm, Encoding.UTF8.GetBytes(data), out var saltData, keySize, iterations);
        salt = Convert.ToBase64String(saltData);
        return hash;
    }

}