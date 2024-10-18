namespace Application.Halpers
{
    public interface ICommand
    { }

    public interface IQuery
    { }

    public interface IResponse
    { }

    public interface IEvent
    { }

    public interface ICommandHandler<C> where C : ICommand
    {
        Task Handle(C command, CancellationToken cancellationToken);
    }

    public interface IQueryHandler<Q, R> where Q : IQuery where R : IResponse
    {
        Task<R> Handle(Q query, CancellationToken cancellationToken);
    }

    public interface IEventHandler<E, R> where E : IEvent where R : IResponse
    {
        Task<R> Handle(E query, CancellationToken cancellationToken);
    }
}