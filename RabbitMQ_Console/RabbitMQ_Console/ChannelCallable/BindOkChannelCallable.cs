#nullable disable

using RabbitMQ.Client;

namespace RabbitMQ_Console.ChannelCallable;

internal class BindOkChannelCallable : ChannelCallableAbstract<object>
{
    private static string USER_INBOXES_EXCHANGE = "user-inboxes";

    public override object Call(IModel channel, int userId = -1, string message = "")
    {
        if (userId == -1)
            return null;

        string queue = GetUserInboxQueue(userId);

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

        channel.QueueBind(queue, USER_INBOXES_EXCHANGE, routingKey);
        return null;
    }

    public override string GetDescriptioin()
        => $"Declaring user queue: {USER_INBOXES_EXCHANGE}";

    private string GetUserInboxQueue(int userId)
        => $"user-inbox.{userId}";

}
