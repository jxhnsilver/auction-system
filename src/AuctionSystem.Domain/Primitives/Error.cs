namespace AuctionSystem.Domain.Primitives
{
    public record Error
    {
        public static readonly Error None = new(string.Empty, ErrorType.None);

        public string Message { get; }
        public ErrorType Type { get; }

        private Error(string message, ErrorType type)
        {
            Message = message;
            Type = type;
        }

        public static Error Validation(string message)
            => new(message, ErrorType.Validation);
        public static Error NotFound(string message)
            => new(message, ErrorType.NotFound);
    }
}
