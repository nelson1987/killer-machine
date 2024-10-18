namespace Domain.Brokers
{
    public interface IMessageBroker
    {
        Task SendMessageAsync(string message);
    }
}