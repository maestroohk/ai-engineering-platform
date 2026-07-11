namespace AiEng.Platform.Application.Projects;

public sealed class ValidationError
{
    public string Code { get; }
    public string Message { get; }

    private ValidationError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public static ValidationError Required(string fieldName) =>
        new("required", $"{fieldName} is required.");

    public static ValidationError InvalidPath(string fieldName, string path) =>
        new("invalid_path", $"{fieldName} is not an existing directory: {path}.");

    public static ValidationError NotFound(string entity, Guid id) =>
        new("not_found", $"{entity} {id} is not registered.");
}

public sealed class Result<T>
{
    public T? Value { get; }

    public ValidationError? Error { get; }

    public bool IsSuccess { get; }

    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
        Error = null;
    }

    private Result(ValidationError error)
    {
        Value = default;
        IsSuccess = false;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(ValidationError error) => new(error);
}
