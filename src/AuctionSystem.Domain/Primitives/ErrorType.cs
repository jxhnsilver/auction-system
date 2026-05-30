namespace AuctionSystem.Domain.Primitives
{
    public enum ErrorType
    {
        None = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Unauthorized = 4
    }
}
