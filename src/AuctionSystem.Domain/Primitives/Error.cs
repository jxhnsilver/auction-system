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

        public static Error Failure(string message)
            => new(message, ErrorType.Failure);
        public static Error NotFound(string message)
            => new(message, ErrorType.NotFound);
        public static Error Conflict(string message)
            => new(message, ErrorType.Conflict);
    }
}
