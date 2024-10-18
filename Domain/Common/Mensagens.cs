namespace Domain.Common;

public static class Mensagens
{
    public static string O_CAMPO_X0_INVALIDO = "O campo {0} é inválido.";
    public static string O_CAMPO_X0_NAO_INFORMADO = "O campo {0} não foi informado.";
    public static string USUARIO_CADASTRADO_COM_SUCESSO = "Usuário cadstrado com sucesso.";
}

public interface IResult
{ }

public class Result<T> where T : IResult
{
    public Result(T data, bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
        Data = data;
    }

    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public T Data { get; }

    public static Result<T> Success(T result) => new(result, true, Error.None);

    public static Result<T> Fail(string code, string description) => new(false, new(code, description));
}

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}

public class Result
{
    private Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);
}