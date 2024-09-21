namespace EcommerceOrderManagement.Domain.Infrastructure;

public readonly struct Result
{
    public string Error { get; }
    public bool IsFailure { get; }
    public bool IsSuccess => !IsFailure;

    private Result(string error, bool isFailure)
    {
        Error = error;
        IsFailure = isFailure;
    }

    public static Result Success() => new Result(string.Empty, false);
    public static Result Failure(string error) => new Result(error, true);
    public static Result Combine(params Result[] results)
    {
        foreach (var result in results)
        {
            if (result.IsFailure)
                return result; // Retorna o primeiro erro
        }

        return Success(); // Se não houver falha, retorna sucesso
    }
}

public readonly struct Result<T>
{
    public T Value { get; }
    public string Error { get; }
    public bool IsFailure { get; }
    public bool IsSuccess => !IsFailure;

    private Result(string error, bool isFailure, T value)
    {
        Error = error;
        IsFailure = isFailure;
        Value = value;
    }

    public static Result<T> Success(T value) => new Result<T>(string.Empty, false, value);
    public static Result<T> Failure(string error) => new Result<T>(error, true, default!);

    // Facilita a conversão implícita de um valor para um Result de sucesso
    public static implicit operator Result<T>(T value) => Success(value);
    
    // Facilita a conversão implícita de um Result para Result<T> em caso de falha
    public static implicit operator Result<T>(Result result) => Failure(result.Error);
    
    public static Result Combine(params Result[] results)
    {
        foreach (var result in results)
        {
            if (result.IsFailure)
                return result;
        }

        return Result.Success();
    }
}
