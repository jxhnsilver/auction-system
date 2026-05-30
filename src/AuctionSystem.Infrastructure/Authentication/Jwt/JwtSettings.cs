namespace AuctionSystem.Infrastructure.Authentication.Jwt
{
    public sealed class JwtSettings
    {
        /// <summary>
        /// Secret key used for signing tokens (minimum 64 characters / 32 bytes)
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Token issuer (who created the token)
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Token audience (who the token is intended for)
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Token lifetime in seconds (default: 3600 seconds = 1 hour)
        /// </summary>
        public int ExpirationInSeconds { get; set; } = 3600;
    }
}
