#nullable disable

using RabbitMQ.Client;
using System.Text;

namespace RabbitMQ_Console.ChannelCallable;

public class SendUserMessageChannelCallable : ChannelCallableAbstract<string>
{
    private static string USER_INBOXES_EXCHANGE = "user-inboxes";
    private static string MESSAGE_CONTENT_TYPE = "application/vnd.ccm.pmsg.v1+json";
    private static string MESSAGE_ENCODING = "UTF-8";

    private long _userId = -1;

    public override string Call(IModel channel, int userId = -1, string message = "")
    {
        if (userId == -1) return string.Empty;
        _userId = userId;

        string queue = GetUserInboxQueue(userId);
        DeclareUserMessageQueue(queue, channel);
        string messageId = Guid.NewGuid().ToString();

        var props = channel.CreateBasicProperties();
        props.ContentType = MESSAGE_CONTENT_TYPE;
        props.ContentEncoding = MESSAGE_ENCODING;
        props.MessageId = messageId;
        props.DeliveryMode = 2;

        string routingKey = queue;

        byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(USER_INBOXES_EXCHANGE, routingKey, props, messageBodyBytes);
        return messageId;
    }

    public override string GetDescriptioin()
        => $"Sending message to user: {_userId}";

    private string GetUserInboxQueue(int userId)
        => $"user-inbox.{userId}";

    private void DeclareUserMessageQueue(string queue, IModel channel)
    {
        // survive a server restart
        bool durable = true;
        // can be consumed by another connection
        bool exclusive = false;
        // keep the queue
        bool autoDelete = false;
        // no special arguments
        Dictionary<string, object> arguments = null;

        channel.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);

        // bind the addressee's queue to the direct exchange
        string routingKey = queue;
        channel.QueueBind(queue, USER_INBOXES_EXCHANGE, routingKey);
    }
}
