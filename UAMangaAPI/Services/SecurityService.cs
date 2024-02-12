using System.Security.Cryptography;

namespace UAMangaAPI.Services
{
    public class SecurityService
    {
        private readonly IServiceProvider _services;

        public SecurityService(IServiceProvider services)
        {
            _services = services;
        }

        public static string HashPassword(string password)
        {
            // Generate a salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // Create the Rfc2898DeriveBytes with the password, salt, and number of iterations
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);

            // Get the hashed password
            byte[] hash = pbkdf2.GetBytes(20);

            // Combine the salt and hashed password
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Convert to Base64 for storage
            string hashedPassword = Convert.ToBase64String(hashBytes);

            return hashedPassword;
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Convert the Base64 string to bytes
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            // Extract the salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Create the Rfc2898DeriveBytes with the provided password, extracted salt, and number of iterations
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);

            // Get the hashed password
            byte[] hash = pbkdf2.GetBytes(20);

            // Compare the hashed password with the stored hash
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
