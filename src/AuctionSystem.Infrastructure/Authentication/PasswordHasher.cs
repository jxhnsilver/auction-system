using AuctionSystem.Application.Abstractions;
using System.Security.Cryptography;

namespace AuctionSystem.Infrastructure.Authentication
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100_000;

        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                Algorithm,
                HashSize);

            return $"{Convert.ToHexString(salt)}-{Convert.ToHexString(hash)}";
        }

        public bool Verify(string password, string storedHash)
        {
            string[] parts = storedHash.Split('-');
            if (parts.Length != 2)
                return false;

            try
            {
                byte[] salt = Convert.FromHexString(parts[0]);
                byte[] hash = Convert.FromHexString(parts[1]);

                byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    Iterations,
                    Algorithm,
                    hash.Length);

                return CryptographicOperations.FixedTimeEquals(hash, inputHash);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
