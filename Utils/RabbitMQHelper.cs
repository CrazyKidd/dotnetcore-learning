using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NetNote.Utils
{
    public class RabbitMQHelper : IMQHelper
    {
        private string _exchangeName;
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public RabbitMQHelper(string exchangeName = "exchangeName")
        {
            _exchangeName = exchangeName;
            _connectionFactory = new ConnectionFactory();
            _connectionFactory.HostName = "127.0.0.1";
            _connectionFactory.UserName = "guest";
            _connectionFactory.Password = "guest";
            //创建连接
            // _connection = _connectionFactory.CreateConnection();
            // //创建通道
            // _channel = _connection.CreateModel();
            // //声明交换机
            // _channel.ExchangeDeclare(exchange: exchangeName, ExchangeType.Topic);

        }
        public void Receive(string queName, Action<string> received)
        {
            //throw new NotImplementedException();
            using (_connection = _connectionFactory.CreateConnection())
            {
                //创建通道
                using (_channel = _connection.CreateModel())
                {
                    //时间消费者
                    EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
                    consumer.Received += async(ch, ea) =>
                      {
                         var  message = Encoding.UTF8.GetString(ea.Body.ToArray());
                         //消费消息
                         await Task.Run(()=>received((message)))
                         .ContinueWith(preT=>{
                          //确认消息已消费
                          //Console.WriteLine("前一个结果是 {0}.",preT.Result);
                         _channel.BasicAck(ea.DeliveryTag,false);
                         });
                                            
                      };
                      //启动消费者 设置为手动应答消息
                      _channel.BasicConsume(queName,false,consumer);
                }
            }

        }

        public void SendMsg<T>(string queName, T msg)
        {
            //throw new NotImplementedException();
            //创建连接
            using (_connection = _connectionFactory.CreateConnection())
            {
                //创建通道
                using (_channel = _connection.CreateModel())
                {
                    _channel.ExchangeDeclare(exchange: _exchangeName, ExchangeType.Topic);
                    _channel.QueueDeclare(queName, true, false, false, null);
                    //绑定队列
                    _channel.QueueBind(queName, exchange: _exchangeName, queName);
                    var basicProperties = _channel.CreateBasicProperties();
                    basicProperties.DeliveryMode = 2;
                    var message = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
                    var address = new PublicationAddress(ExchangeType.Direct, _exchangeName, queName);
                    _channel.BasicPublish(address, basicProperties, message);
                }
            }
        }
    }
}
