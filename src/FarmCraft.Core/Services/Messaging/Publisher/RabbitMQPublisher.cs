using FarmCraft.Core.Messages;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace FarmCraft.Core.Services.Messaging.Publisher
{
    public class RabbitMQPublisher : IMessagePublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQOptions _options;

        public RabbitMQPublisher(RabbitMQOptions options)
        {
            _options = options;

            var factory = new ConnectionFactory() { HostName = options.Host };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                exchange: options.Exchange, 
                type: options.ExchangeType
            );
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }

        public Task PublishMessage(FarmCraftMessage message)
        {
            var messageBody = JsonConvert.SerializeObject(message);
            byte[] body = Encoding.UTF8.GetBytes(messageBody);

            _channel.BasicPublish(
                exchange: _options.Exchange,
                routingKey: _options.RoutingKey,
                basicProperties: null,
                body: body
            );

            return Task.CompletedTask;
        }

        public Task PublishMessageBatch(List<IMessage> messages, int batchSize)
        {
            throw new NotImplementedException();
        }
    }
}
