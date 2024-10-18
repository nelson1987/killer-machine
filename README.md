# killer-machine
### https://www.youtube.com/watch?v=9bEMrFIt-aY&ab_channel=BrunoBrito

## Curso de Arquitetura utilizando: TDD + DDD + CQRS

### Conhecendo TDD
#### O desenvolvimento orientado por testes depende da repetição de um ciclo incrivelmente curto. Este ciclo é composto de três fases:

- Primeiro, você escreve um teste que representa um requisito específico da funcionalidade que você está tentando implementar.
- Em seguida, você faz o teste passar, escrevendo a quantidade mínima de código de produção com a qual você pode escapar.
- Se necessário, você refatoria o código para eliminar duplicações ou outros problemas.

### Comandos para criar solução no .NetCore
```sh
dotnet new sln -n Preacher # Criar Solution
dotnet new xunit -n Preacher.UnitTests # Criar Camada de testes unitários
dotnet new gitignore # Criar .gitignore
ls -la
dotnet sln add **/*.csproj
```
### Commandos GIT
``` sh
# Commit normal
git status
git add .
git commit -m "Criacao de solution"
git push
git commit -am "Criacao de solution" # Alterar nome do Commit
git log --oneline # ver histórico de commits
git rebase -i [id do commit] # Rebase
```
### Extensão do VSCode
#### Visual Studio Keymap
 
### Teste Unitários

#### Criar Entidade Utilizando Setter Privado e com Construtor
#### Criar Teste Unitário de Entidade

#### Criar Contexto
#### Criar Teste Unitário de Contexto(DbSet<Entidade>) da Entidade

#### Criar DomainEvent
#### Criar Teste Unitário de DomainEvent

#### Criar Repository
#### Criar Teste Unitário de Repository

#### Criar Handler
#### Criar Teste Unitário do Handler

#### Criar Command
#### Criar Teste Unitário do Command

#### Criar Response
#### Criar Teste Unitário do Response

#### Criar Validator
#### Criar Teste Unitário do Validator
