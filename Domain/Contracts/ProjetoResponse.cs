using Domain.Common;

namespace Domain.Contracts
{
    public record ProjetoResponse(int Id, string Nome) : IResult;
}