using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly Contexto _contexto;

    public UsuarioRepository(Contexto contexto)
    {
        _contexto = contexto;
    }

    public async Task AdicionarAsync(Usuario usuario)
    {
        await _contexto.Set<Usuario>().AddAsync(usuario);
        await _contexto.SaveChangesAsync();
    }

    public async Task<Usuario> BuscarPorId(int idUsuario) =>
        await _contexto.Set<Usuario>().FirstAsync(x => x.Id == idUsuario);

    public async Task<List<Usuario>> ListarAsync() => await _contexto.Set<Usuario>().ToListAsync();
}