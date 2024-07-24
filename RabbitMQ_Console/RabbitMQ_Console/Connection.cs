#nullable disable

using RabbitMQ.Client;

namespace RabbitMQ_Console;

internal class Connection
{
    internal static ConnectionFactory GetConnectionFactory()
    {
        return new()
        {
            UserName = "dev",
            Password = "dev123",
            VirtualHost = "dev_vhost",
            HostName = "localhost",
            Port = 15672,
        };        
    }
}
