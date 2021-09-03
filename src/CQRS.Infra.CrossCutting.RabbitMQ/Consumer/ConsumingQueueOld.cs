using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CQRS.Infra.CrossCutting.RabbitMQ.Consumer
{
    [Obsolete]
    public class ConsumingQueueOld : BackgroundService
    {
        private IServiceProvider _sp;
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;

        public ConsumingQueueOld(IServiceProvider sp)
        {
            _sp = sp;
            _factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            _factory.UserName = "guest";
            _factory.Password = "guest";
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "FILA_TESTE", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public string ReadQueue()
        {
            var message = string.Empty;

            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            factory.UserName = "guest";
            factory.Password = "guest";
            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();
            channel.QueueDeclare(queue: "FILA_TESTE",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                     message = Encoding.UTF8.GetString(body);
                    //Console.WriteLine(" [x] Recebendo do Rabbit: {0}", message);
                    //Console.WriteLine("-----------------------------------");

                };
            channel.BasicConsume(queue: "FILA_TESTE",
                                            autoAck: true,
                                            consumer: consumer);
            return message;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                _channel.Dispose();
                _connection.Dispose();

                return Task.CompletedTask;
            }

            var consumer = new EventingBasicConsumer(_channel);

            //consumer.Received += (model, ea) =>
            //{
            //    var body = ea.Body.ToArray();
            //    var message = Encoding.UTF8.GetString(body);
            //    Console.WriteLine(" [x] Received {0}", message);

            //    Task.Run(() =>
            //    {
            //        var chunks = message.Split(".");
                   

            //        var hero = new Customer();
            //        if (chunks.Length == 5)
            //        {
            //            hero.Name = chunks[1];
            //            hero.Powers = chunks[2];
            //            hero.HasCape = chunks[3] == "1";
            //            hero.IsAlive = chunks[5] == "1";
            //            hero.Category = Enum.Parse<Category>(chunks[6]);
            //        }

            //        using (var scope = _sp.CreateScope())
            //        {
            //            var db = scope.ServiceProvider.GetRequiredService<ICustomeRepos>();
            //            db.Create(hero);
            //        }
            //    });
            //};

            //_channel.BasicConsume(queue: "heroes", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
