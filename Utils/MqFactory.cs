using System;
using RabbitMQ.Client;

namespace NetNote.Utils
{
    public class MqFactory
    {
        private readonly ConnectionFactory _connectionFactory;
        public MqFactory(){
            _connectionFactory=new ConnectionFactory();
            _connectionFactory.HostName="127.0.0.1";
            _connectionFactory.UserName="guest";
            _connectionFactory.Password="guest";
        }
    }
}
