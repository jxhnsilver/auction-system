namespace AuctionSystem.Infrastructure.Authentication.Jwt
{
    public static class JwtSettingsValidator
    {
        private const int MinSecretKeyLength = 64;

        public static void Validate(JwtSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.SecretKey))
                throw new InvalidOperationException("JWT SecretKey is not configured");

            if (settings.SecretKey.Length < MinSecretKeyLength)
                throw new InvalidOperationException($"JWT SecretKey must be at least {MinSecretKeyLength} characters");

            if (string.IsNullOrWhiteSpace(settings.Issuer))
                throw new InvalidOperationException("JWT Issuer is not configured");

            if (string.IsNullOrWhiteSpace(settings.Audience))
                throw new InvalidOperationException("JWT Audience is not configured");

            if (settings.ExpirationInSeconds <= 0)
                throw new InvalidOperationException("JWT ExpirationInSeconds must be greater than 0");
        }
    }
}
