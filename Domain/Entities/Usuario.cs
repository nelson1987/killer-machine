using Domain.Contracts;

namespace Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public Usuario()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Empregados onde VALIDAMOS AS REGRAS DE CRIAÇÃO DE ENTIDADE
        /// </summary>
        /// <param name="request"></param>
        public Usuario(CriacaoUsuarioCommand request)
        {
            Id = 0;
            Nome = request.Nome;
            ArgumentException.ThrowIfNullOrEmpty(Nome);//, Mensagens.O_CAMPO_X0_INVALIDO);
        }

        public string Nome { get; private set; }
    }
}