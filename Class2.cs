using System.Collections.ObjectModel;

namespace MangaBank.UnitTests
{
    /*
     Status
     * Um pedido inicia com o status "Pendente de pagamento".
     * Após pago, o pedido muda seu status para "Pronto para envio", e é direcionado para a transportadora.
     * Quando enviado para a transportadora, o pedido muda seu status para "Em trânsito".
     * Casos de Uso
     * Existem alguns processos de negócios que precisamos dar suporte:
     * Pagar Pedido - Aceitar pagamentos para um pedido
     * Gerir Itens do Pedido - Adicionar e remover itens em um pedido e atualizar a quantidade de um item do pedido
     * Gerir Status do Pedido - Gerenciar o status do pedido e poder enviar um pedido a um cliente
     */

    public class PedidoUnitTests
    {
        public enum StatusPedido
        {
            PendentePagamento,
            ProntoEnvio,
            EmTransito
        }

        public class Pedido
        {
            public StatusPedido Status { get; private set; }

            internal void Pagar()
            {
                Status = StatusPedido.ProntoEnvio;
            }

            internal void EnviarTransportadora()
            {
                Status = StatusPedido.EmTransito;
            }
        }

        [Fact]
        public void Dado_Pedido_Quando_Instanciado_Entao_StatusDeveSerPendentePagamento()
        {
            var pedido = new Pedido();
            Assert.Equal(StatusPedido.PendentePagamento, pedido.Status);
        }

        [Fact]
        public void Dado_Pedido_Quando_PagarInvocado_Entao_StatusDeveSerProntoEnvio()
        {
            var pedido = new Pedido();
            pedido.Pagar();
            Assert.Equal(StatusPedido.ProntoEnvio, pedido.Status);
        }

        [Fact]
        public void Dado_Pedido_Quando_EnviarTransportadoraInvocado_Entao_StatusDeveSerProntoEnvio()
        {
            var pedido = new Pedido();
            pedido.EnviarTransportadora();
            Assert.Equal(StatusPedido.EmTransito, pedido.Status);
        }

        //
    }

    public enum OrderStatus
    {
        PendingPayment,
        ReadyForShipping,
        InTransit
    }

    public class Order
    {
        internal List<OrderItem> _items = new();
        public long OrderId { get; private set; }
        public DateTime CreationDate { get; private set; }
        public DateTime? ShippingDate { get; private set; }
        public decimal PaidAmount { get; private set; }
        public OrderStatus Status { get; private set; }

        public Order()
        {
            Status = OrderStatus.PendingPayment;
            CreationDate = DateTime.Now;
        }

        public IReadOnlyCollection<OrderItem> Items => new ReadOnlyCollection<OrderItem>(_items);

        public decimal OrderTotal => _items.Sum(x => Convert.ToDecimal(x.Quantity) * x.UnitPrice);

        public void AddPayment(decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Amount must be positive.");

            if (amount > OrderTotal - PaidAmount)
                throw new InvalidOperationException("Payment can't exceed order total");

            PaidAmount += amount;
            if (PaidAmount >= OrderTotal)
                Status = OrderStatus.ReadyForShipping;
        }

        public void AddItem(string itemName, int quantity, decimal unitPrice)
        {
            if (Status != OrderStatus.PendingPayment)
                throw new InvalidOperationException("Can't modify order once payment has been done.");
            _items.Add(new OrderItem(itemName, quantity, unitPrice));
        }

        public void RemoveItem(string itemName)
        {
            if (Status != OrderStatus.PendingPayment)
                throw new InvalidOperationException("Can't modify order once payment has been done.");
            _items.RemoveAll(x => x.ItemName == itemName);
        }

        public void AddQuantity(string itemName, int quantity)
            => _items.Find(x => x.ItemName.Equals(itemName))?.AddQuantity(quantity);

        public void WithdrawQuantity(string itemName, int quantity)
            => _items.Find(x => x.ItemName.Equals(itemName))?.WithdrawQuantity(quantity);

        public void ShipOrder()
        {
            if (_items.Sum(x => x.Quantity) <= 0)
                throw new InvalidOperationException("Can´t ship an order with no items.");
            if (Status == OrderStatus.PendingPayment)
                throw new InvalidOperationException("Can´t ship order unpaid order.");
            if (Status == OrderStatus.InTransit)
                throw new InvalidOperationException("Order already shipped to customer.");
            ShippingDate = DateTime.Now;
            Status = OrderStatus.InTransit;
        }
    }

    public class OrderItem
    {
        private OrderItem()
        { }

        internal OrderItem(string itemName, int quantity, decimal unitPrice)
        {
            if (string.IsNullOrEmpty(itemName))
                throw new ArgumentException($"'{nameof(itemName)}' cannot be null or empty.", nameof(itemName));
            if (quantity == 0)
                throw new ArgumentException("Quantity must be at least one.", nameof(quantity));
            if (unitPrice <= 0)
                throw new ArgumentException("Unit price must be above zero.", nameof(unitPrice));

            ItemName = itemName;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public long OrderItemId { get; private set; }
        public string ItemName { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        internal void AddQuantity(int quantity)
        {
            this.Quantity += quantity;
        }

        internal void WithdrawQuantity(int quantity)
        {
            if (this.Quantity - quantity <= 0)
                throw new InvalidOperationException("Can't remove all units. Remove the entire item instead.");
            this.Quantity -= quantity;
        }
    }

    public interface IOrdersService
    {
        Task AddAmountDiscountAsync(Order order, string description, decimal amount);

        Task<OrderItem> AddOrderItemAsync(Order order, string name, decimal price, int units);

        Task AddPercentageDiscountAsync(Order order, string description, decimal percentage);

        Task CancelOrderAsync(Order order);

        Task<Order> CreateOrderAsync();

        Task SendToCustomerAsync(Order order);
    }
}