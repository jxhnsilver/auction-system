using AuctionSystem.Domain.Primitives;
using AuctionSystem.Domain.Users.Errors;
using System.Text.RegularExpressions;

namespace AuctionSystem.Domain.Users
{
    public sealed record Email
    {
        private static readonly Regex EmailRegex = new(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private const int MaxLocalLength = 64; // RFC 5321

        public const int MaxTotalLength = 254; // RFC 5321
        public const int MinEmailLength = 3; // RFC 5322

        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Result<Email> Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result<Email>.Failure(EmailErrors.Empty);

            var normalized = Normalize(email);

            if (!IsValid(normalized))
                return Result<Email>.Failure(EmailErrors.InvalidFormat);

            return Result<Email>.Success(new Email(normalized));
        }

        private static bool IsValid(string email)
        {
            if (email.Length < MinEmailLength || email.Length > MaxTotalLength)
                return false;

            if (!EmailRegex.IsMatch(email))
                return false;

            var atIndex = email.IndexOf('@');

            if (atIndex > MaxLocalLength)
                return false;

            return true;
        }

        private static string Normalize(string email)
            => email.Trim().ToLowerInvariant();

        public override string ToString() => Value;
    }
}
