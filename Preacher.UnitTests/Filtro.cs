using FluentAssertions;
using System.Linq;

namespace Preacher.UnitTests
{
    public record Projeto(string Nome, DateTime Criacao, TipoProjetoEnum Tipo, StatusProjetoEnum Status);

    public enum TipoProjetoEnum
    { Aberto = 1, EmExecucao = 2, Fechado = 3 };

    public enum StatusProjetoEnum
    { Aberto = 1, EmExecucao = 2, Fechado = 3 };

    public class FiltroUniTests
    {
        private readonly List<Projeto> _projeto;

        public FiltroUniTests()
        {
            _projeto = new List<Projeto>() {
                new Projeto("João", DateTime.UtcNow.AddDays(-1), TipoProjetoEnum.Aberto, StatusProjetoEnum.Aberto),
                new Projeto("Jefferson", DateTime.UtcNow, TipoProjetoEnum.EmExecucao, StatusProjetoEnum.EmExecucao)
            };
        }

        [Fact]
        public void FiltrarProjetoPorNome()
        {
            // Act
            var resultado = GetList("J", DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(2)
                , new[] { TipoProjetoEnum.Aberto, TipoProjetoEnum.EmExecucao }
                , new[] { StatusProjetoEnum.Aberto, StatusProjetoEnum.EmExecucao });
            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().ContainSingle(p => p.Nome == "João");
            resultado.Should().ContainSingle(p => p.Nome == "Jefferson");
        }

        [Fact]
        public void FiltrarProjetoFiltroPorStatusAberto()
        {
            // Act
            var resultado = GetList("J", DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(2)
                , new[] { TipoProjetoEnum.Aberto, TipoProjetoEnum.EmExecucao }
                , new[] { StatusProjetoEnum.Aberto });
            // Assert
            resultado.Should().HaveCount(1);
            resultado.Should().ContainSingle(p => p.Nome == "João");
        }

        [Fact]
        public void FiltrarProjetoSemNome()
        {
            // Act
            var resultado = GetList(string.Empty, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(2)
                , new[] { TipoProjetoEnum.Aberto, TipoProjetoEnum.EmExecucao }
                , new[] { StatusProjetoEnum.Aberto, StatusProjetoEnum.EmExecucao });
            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().ContainSingle(p => p.Nome == "João");
            resultado.Should().ContainSingle(p => p.Nome == "Jefferson");
        }

        private IEnumerable<Projeto> GetList(string nomeFiltro, DateTime? dataInicial, DateTime? dataFinal, TipoProjetoEnum[] tipos, StatusProjetoEnum[] status)
        {
            List<Predicate<Projeto>> filtros = new List<Predicate<Projeto>>();

            if (!string.IsNullOrWhiteSpace(nomeFiltro))
                filtros.Add(x => x.Nome.Contains(nomeFiltro));

            if (dataInicial != null)
                filtros.Add(x => x.Criacao >= dataInicial);

            if (dataFinal != null)
                filtros.Add(x => x.Criacao <= dataFinal);

            if (tipos.Length != 0)
                filtros.Add(x => tipos.Contains(x.Tipo));

            if (status.Length != 0)
                filtros.Add(x => status.Contains(x.Status));

            List<Projeto> resultado = _projeto.FindAll(filtros);
            return resultado;
        }
    }

    public static class ExtensionMethods
    {
        public static List<T> FindAll<T>(this List<T> list, List<Predicate<T>> predicates)
        {
            List<T> L = new List<T>();
            foreach (T item in list)
            {
                bool pass = true;
                foreach (Predicate<T> p in predicates)
                {
                    if (!(p(item)))
                    {
                        pass = false;
                        break;
                    }
                }
                if (pass) L.Add(item);
            }
            return L;
        }
    }
}