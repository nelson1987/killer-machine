using Domain.Entities;

namespace Domain.Repositories
{
    public interface IUsuarioRepository
    {
        Task AdicionarAsync(Usuario usuario);

        Task<List<Usuario>> ListarAsync();
    }
}