using System.Security.Cryptography;
using System.Text;

namespace CardManager.Application.Services.Cryptography
{
    public static class PasswordEncripter
    {
        public static string EncryptPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = SHA512.HashData(bytes);

            return StringBytes(hashBytes);
        }

        public static bool VerifyPassword(string password, string passwordHash)
        {
            var passwordEncriptografy = EncryptPassword(password);

            return (passwordEncriptografy == passwordHash);
        }

        private static string StringBytes(byte[] bytes)
        {
            var stringBuilder = new StringBuilder();
            foreach (var b in bytes)
            {
                stringBuilder.Append(b.ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    }
}
