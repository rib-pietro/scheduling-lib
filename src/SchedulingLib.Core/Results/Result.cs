namespace SchedulingLib.Core.Results;

/// <summary>
/// Represents the outcome of an operation that produces no value.
/// </summary>
public record Result
{
    /// <summary>Gets whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets the error message when the operation failed; null on success.</summary>
    public string? ErrorMessage { get; }

    private Result(bool isSuccess, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    /// <summary>Creates a successful result.</summary>
    public static Result Ok() => new(true, null);

    /// <summary>Creates a failed result with <paramref name="errorMessage"/>.</summary>
    public static Result Fail(string errorMessage) => new(false, errorMessage);

    /// <summary>Creates a successful result carrying <paramref name="value"/>.</summary>
    public static Result<T> Ok<T>(T value) => Result<T>.Ok(value);

    /// <summary>Creates a failed result of type <typeparamref name="T"/> with <paramref name="errorMessage"/>.</summary>
    public static Result<T> Fail<T>(string errorMessage) => Result<T>.Fail(errorMessage);
}

/// <summary>
/// Represents the outcome of an operation that produces a value of type <typeparamref name="T"/>.
/// </summary>
public record Result<T>
{
    /// <summary>Gets the result value; default when the operation failed.</summary>
    public T? Value { get; }

    /// <summary>Gets whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets the error message when the operation failed; null on success.</summary>
    public string? ErrorMessage { get; }

    private Result(T? value, bool isSuccess, string? errorMessage)
    {
        Value = value;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    /// <summary>Creates a successful result carrying <paramref name="value"/>.</summary>
    public static Result<T> Ok(T value) => new(value, true, null);

    /// <summary>Creates a failed result with <paramref name="errorMessage"/>.</summary>
    public static Result<T> Fail(string errorMessage) => new(default, false, errorMessage);
}
