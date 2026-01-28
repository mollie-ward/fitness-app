namespace FitnessApp.Application.Common.Models;

/// <summary>
/// Represents a standardized API response
/// </summary>
/// <typeparam name="T">Type of the response data</typeparam>
public class Result<T>
{
    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// The data returned by the operation
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Error messages if the operation failed
    /// </summary>
    public string[]? Errors { get; set; }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static Result<T> Success(T data)
    {
        return new Result<T> { Succeeded = true, Data = data };
    }

    /// <summary>
    /// Creates a failed result
    /// </summary>
    public static Result<T> Failure(params string[] errors)
    {
        return new Result<T> { Succeeded = false, Errors = errors };
    }
}
