using Application.Halpers;
using Domain.Contracts;
using Domain.Services;

namespace Application.UseCases.CriarProjetos;
public record CriacaoProjetoCommand(string Nome) : ICommand;

public class CriacaoProjetoHandler : ICommandHandler<CriacaoProjetoCommand>
{
    private readonly IProjetoService _usuarioService;

    public CriacaoProjetoHandler(IProjetoService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    public async Task Handle(CriacaoProjetoCommand command, CancellationToken cancellationToken)
    {
        CriacaoProjetoRequest usuario = new CriacaoProjetoRequest(command.Nome);
        await _usuarioService.AdicionarAsync(usuario);
    }
}

public record ProjetoCriadoEvent() : IEvent;
public record ProjetoCriadoResponse() : IResponse;

public class ProjetoCriadoHandler : IEventHandler<ProjetoCriadoEvent, ProjetoCriadoResponse>
{
    public Task<ProjetoCriadoResponse> Handle(ProjetoCriadoEvent query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}