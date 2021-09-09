namespace CQRS.Application.Services
{

    //public class ConsumingQueueService : BackgroundService
    //{ 
    //    private IServiceProvider _sp;
    //    private ConnectionFactory _factory;
    //    private IConnection _connection;
    //    private IModel _channel;

    //    private readonly IMapper _mapper;
    //    private readonly IServiceScopeFactory _serviceScopeFactory;

    //    public ConsumingQueueService(IServiceProvider sp, IServiceScopeFactory serviceScopeFactory, IMapper mapper)
    //    {
    //        _sp = sp;
    //        _factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
    //        _factory.UserName = "guest";
    //        _factory.Password = "guest";
    //        _connection = _factory.CreateConnection();
    //        _channel = _connection.CreateModel();
    //        _channel.QueueDeclare(queue: "FILA_TESTE", durable: false, exclusive: false, autoDelete: false, arguments: null);

    //        _mapper = mapper;
    //        _serviceScopeFactory = serviceScopeFactory;
    //    }

    //    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        if (stoppingToken.IsCancellationRequested)
    //        {
    //            _channel.Dispose();
    //            _connection.Dispose();

    //            return Task.CompletedTask;
    //        }

    //        var consumer = new EventingBasicConsumer(_channel);

    //        consumer.Received += (model, ea) =>
    //        {
    //            var body = ea.Body.ToArray();
    //            var message = Encoding.UTF8.GetString(body);
    //            Console.WriteLine(" [x] Received {0}", message);

    //            Task.Run(() =>
    //            {
    //                var chunks = message.Split(".");

    //                //var customer = JsonSerializer.Deserialize<CustomerViewModel>(message);
    //                //_customerAppService.Register(customer);

    //                //https://bartwullems.blogspot.com/2019/11/using-scoped-service-inside.html
    //                using (var scope = _serviceScopeFactory.CreateScope())
    //                {
    //                    var appService = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();

    //                    var customerVM = JsonSerializer.Deserialize<CustomerViewModel>(message);

    //                    var customerLocal = appService.GetById(customerVM.Id);

    //                    var customer = _mapper.Map<Customer>(customerVM);

    //                    if (customerLocal.Result == null)
    //                    {
    //                        appService.Add(customer, TypeDB.StoreRead);
    //                    }
    //                    else {
    //                        appService.Update(customer, TypeDB.StoreRead);
    //                    }






    //                    //appService.Update(customer);
    //                    //commit.Commit();

    //                    //Do something here
    //                }



    //            });
    //        };

    //        _channel.BasicConsume(queue: "FILA_TESTE", autoAck: true, consumer: consumer);

    //        return Task.CompletedTask;
    //    }


    //}
}
