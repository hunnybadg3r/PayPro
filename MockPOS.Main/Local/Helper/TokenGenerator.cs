using System.Security.Cryptography;

namespace MockPOS.Main.Local.Helper
{
    public class TokenGenerator
    {
        private const string ALLOWED_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static string GenerateToken(int length = 16)
        {
            var result = new char[length];
            var bytes = new byte[sizeof(uint)];

            using (var rng = RandomNumberGenerator.Create())
            {
                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(bytes);
                    uint num = BitConverter.ToUInt32(bytes, 0);
                    result[i] = ALLOWED_CHARS[(int)(num % (uint)ALLOWED_CHARS.Length)];
                }
            }

            return new string(result);
        }
    }
}
