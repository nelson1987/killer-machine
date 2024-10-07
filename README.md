# killer-machine
## Curso de Arquitetura utilizando: TDD + DDD + CQRS

### Conhecendo TDD
#### O desenvolvimento orientado por testes depende da repetição de um ciclo incrivelmente curto. Este ciclo é composto de três fases:

- Primeiro, você escreve um teste que representa um requisito específico da funcionalidade que você está tentando implementar.
- Em seguida, você faz o teste passar, escrevendo a quantidade mínima de código de produção com a qual você pode escapar.
- Se necessário, você refatoria o código para eliminar duplicações ou outros problemas.

dotnet new sln -n Preacher
dotnet new xunit -n Preacher.UnitTests
dotnet new gitignore

git status
git add .
git commit -m "Criacao de solution"