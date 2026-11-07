using System.Collections.Generic;

namespace CajaApp.Domain.Shared;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error? Error { get; }

    private Result(bool isSuccess, T? value, Error? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Ok(T value) => new(true, value, null);

    public static Result<T> Fail(Error error) => new(false, default, error);

    public static Result<T> Fail(string errorCode, string errorMessage, IDictionary<string, string[]>? details = null)
        => new(false, default, new Error(errorCode, errorMessage, details));

    public static implicit operator Result<T>(T value) => Ok(value);

    public static implicit operator Result<T>(Error error) => Fail(error);
}

public class Result
{
    public bool IsSuccess { get; }
    public Error? Error { get; }

    private Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Ok() => new(true, null);

    public static Result Fail(Error error) => new(false, error);

    public static Result Fail(string errorCode, string errorMessage, IDictionary<string, string[]>? details = null)
        => new(false, new Error(errorCode, errorMessage, details));

    public static implicit operator Result(Error error) => Fail(error);
}

public class Error
{
    public string Code { get; }
    public string Message { get; }
    public IDictionary<string, string[]>? Details { get; }

    public Error(string code, string message, IDictionary<string, string[]>? details = null)
    {
        Code = code;
        Message = message;
        Details = details;
    }

    public override string ToString() => $"[{Code}] {Message}";
}
