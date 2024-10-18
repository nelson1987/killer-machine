using Application.Halpers;

namespace Application.UseCases.ObterProjetos;
public record ListagemProjetoQuery() : IQuery;
public record ListagemProjetoResponse() : IResponse;

public class ListagemProjetoHandler : IQueryHandler<ListagemProjetoQuery, ListagemProjetoResponse>
{
    public Task<ListagemProjetoResponse> Handle(ListagemProjetoQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}