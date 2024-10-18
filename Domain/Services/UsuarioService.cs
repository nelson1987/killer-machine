using Domain.Brokers;
using Domain.Common;
using Domain.Contracts;
using Domain.Entities;
using Domain.Repositories;

namespace Domain.Services
{
    public interface IUsuarioService
    {
        Task<Result<UsuarioResponse>> AdicionarAsync(CriacaoUsuarioCommand command);
    }

    public class UsuarioService : ServiceBase, IUsuarioService
    {
        private readonly IUsuarioRepository _repositorio;

        public UsuarioService(IUsuarioRepository repositorio, IMessageBroker broker) : base(broker)
        {
            _repositorio = repositorio;
        }

        /// <summary>
        /// Método de adicionar Empregado, nesse método validamos as REGRAS DE NEGÓCIO
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<Result<UsuarioResponse>> ListarAsync()
        {
            var listagem = await _repositorio.ListarAsync();
            return Result<UsuarioResponse>.Success(new UsuarioResponse(0, "usuario.Nome"));
        }

        public async Task<Result<UsuarioResponse>> AdicionarAsync(CriacaoUsuarioCommand command)
        {
            if (string.IsNullOrEmpty(command.Nome))
                return await ValidarCampo(nameof(command.Nome), string.Format(Mensagens.O_CAMPO_X0_NAO_INFORMADO, nameof(command.Nome)));

            var usuario = new Usuario(command);
            await _repositorio.AdicionarAsync(usuario);
            await _broker.SendMessageAsync(Mensagens.USUARIO_CADASTRADO_COM_SUCESSO);

            return Result<UsuarioResponse>.Success(new UsuarioResponse(usuario.Id, usuario.Nome));
        }

        private async Task<Result<UsuarioResponse>> ValidarCampo(string campo, string descricao)
        {
            var mensagem = string.Format(descricao, campo);
            await AddError(mensagem);
            return Result<UsuarioResponse>.Fail(campo, mensagem);
        }
    }
}