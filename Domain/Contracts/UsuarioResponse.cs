using Domain.Services;

namespace Domain.Contracts
{
    public record UsuarioResponse(int Id, string Nome) : IResult;
}