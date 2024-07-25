using RabbitMQ.Client;

namespace RabbitMQ_Console.ChannelCallable;

public abstract class ChannelCallableAbstract<T> where T : class
{
    public abstract string GetDescriptioin();
    public abstract T Call(IModel channel, int userId = -1, string message = "");
    public abstract T Call(IModel channel, int userId = -1, List<string> subscribes = null, List<string> unsubscribes = null, string topic = "", string message = "");
}
