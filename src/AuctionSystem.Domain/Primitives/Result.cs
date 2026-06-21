namespace AuctionSystem.Domain.Primitives
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        private Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None)
                throw new InvalidOperationException("Success result cannot have error");

            if (!isSuccess && error == Error.None)
                throw new InvalidOperationException("Failure result must have error");

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error error) => new(false, error);
    }

    public class Result<T>
    {
        private readonly T? _value;

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }
        public T Value
        {
            get
            {
                if (IsFailure)
                    throw new InvalidOperationException("Cannot access value of failed result");

                return _value!;
            }
        }

        private Result(T value, bool isSuccess, Error error)
        {
            if (isSuccess)
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value), "Success result cannot have a null value");

                if (error != Error.None)
                    throw new InvalidOperationException("Successful result cannot have an error");
            }
            else
            {
                if (error == Error.None)
                    throw new InvalidOperationException("Failed result must have an error");
            }

            _value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result<T> Success(T value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value), "Success result cannot have null value");

            return new Result<T>(value, true, Error.None);
        }
        public static Result<T> Failure(Error error) => new(default!, false, error);
    }
}
