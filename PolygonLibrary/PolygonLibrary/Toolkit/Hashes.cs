using System.Security.Cryptography;
using System.Text;

namespace PolygonLibrary.Toolkit;

public class Hashes {

  /// <summary>
  /// Computes the MD5 hash of the given input string.
  /// </summary>
  /// <param name="input">The input string to compute the hash for.</param>
  /// <returns>A lowercase hexadecimal string representation of the MD5 hash.</returns>
  public static string GetMD5Hash(string input)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = MD5.HashData(inputBytes);

        return Convert.ToHexString(hashBytes).ToLower();
    }

}
