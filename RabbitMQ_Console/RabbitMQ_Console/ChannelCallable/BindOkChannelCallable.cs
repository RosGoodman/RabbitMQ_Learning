#nullable disable

using RabbitMQ.Client;

namespace RabbitMQ_Console.ChannelCallable;

internal class BindOkChannelCallable : ChannelCallableAbstract<object>
{
    private string _currentTypeExchange = string.Empty;
    private int _userId = -1;

    public override object Call(IModel channel, int userId = -1, string message = "")
    {
        if (userId == -1)
            return null;

        string queue = GetUserInboxQueue();
        _currentTypeExchange = Program.USER_INBOXES_EXCHANGE;
        _userId = userId;

        // survive a server restart
        bool durable = true;
        // keep the queue
        bool autoDelete = false;
        // can be consumed by another connection
        bool exclusive = false;
        // no special arguments
        Dictionary<string, object> arguments = null;

        channel.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);

        // bind the addressee's queue to the direct exchange
        string routingKey = queue;

        channel.QueueBind(queue, _currentTypeExchange, routingKey);
        return null;
    }

    public override object Call(IModel channel, int userId, List<string> subscribes = null, List<string> unsubscribes = null, string topic = "", string message = "")
    {
        if (userId == -1)
            return null;

        string queue = GetUserInboxQueue();
        _currentTypeExchange = Program.USER_TOPICS_EXCHANGE;
        _userId = userId;

        // survive a server restart
        bool durable = true;
        // keep the queue
        bool autoDelete = false;
        // can be consumed by another connection
        bool exclusive = false;
        // no special arguments
        Dictionary<string, object> arguments = null;

        channel.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);

        // bind the addressee's queue to the direct exchange
        string routingKey = queue;

        channel.QueueBind(queue, _currentTypeExchange, routingKey);
        return null;
    }

    public override string GetDescriptioin()
        => $"Declaring user queue: {_currentTypeExchange}";

    private string GetUserInboxQueue()
        => $"user-inbox.{_userId}";

}
