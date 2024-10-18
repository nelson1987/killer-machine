using Domain.Brokers;

namespace Domain.Services
{
    public class ServiceBase
    {
        protected readonly IMessageBroker _broker;

        public ServiceBase(IMessageBroker broker)
        {
            _broker = broker;
        }

        public async Task AddError(string mensagem)
        {
            await _broker.SendMessageAsync(mensagem);
        }
    }
}