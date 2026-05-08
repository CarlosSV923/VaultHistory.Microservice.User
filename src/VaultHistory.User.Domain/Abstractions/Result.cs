namespace VaultHistory.User.Domain.Abstractions
{
    public class Result
    {
        public bool IsSucceeded { get; }

        public bool IsFailure => !IsSucceeded;
        public Error Error { get; }

        protected internal Result(bool isSucceeded, Error error)
        {
            if (isSucceeded && error != Error.None)
            {
                throw new InvalidOperationException("A result cannot be successful and contain an error");
            }

            if (!isSucceeded && error == Error.None)
            {
                throw new InvalidOperationException("A failing result needs to contain an error description");
            }

            IsSucceeded = isSucceeded;
            Error = error;
        }

        public static Result Success()
        {
            return new(true, Error.None);
        }

        public static Result<T> Success<T>(T value) => new(true, value, Error.None);

        public static Result Failure(Error error) => new(false, error);

        public static Result<T> Failure<T>(Error error) => new(false, default!, error);

        public static Result<T> Create<T>(T value)
        {
            return value is not null ? Success(value) : Failure<T>(Error.NullValue);
        }
    }

    public class Result<T> : Result
    {
        private T? _value { get; }

        protected internal Result(bool isSucceeded, T value, Error error) : base(isSucceeded, error)
        {
            _value = value;
        }

        public T Value
        {
            get
            {
                if (IsFailure)
                {
                    throw new InvalidOperationException("Cannot access value for a failed result");
                }

                return _value!;
            }
        }

        public static implicit operator Result<T>(T value) => Create(value);
    }
}