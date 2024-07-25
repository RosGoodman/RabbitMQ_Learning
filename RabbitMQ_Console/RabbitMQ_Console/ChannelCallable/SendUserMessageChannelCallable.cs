#nullable disable

using RabbitMQ.Client;
using System.Text;

namespace RabbitMQ_Console.ChannelCallable;

public class SendUserMessageChannelCallable : ChannelCallableAbstract<string>
{
    private string _currentTypeExchange = string.Empty;
    private int _userId = -1;
    private string _topic = string.Empty;

    public override string Call(IModel channel, int userId = -1, string message = "")
    {
        if (userId == -1) return string.Empty;
        _userId = userId;
        _currentTypeExchange = Program.USER_INBOXES_EXCHANGE;

        string queue = GetUserInboxQueue(userId);
        DeclareUserMessageQueue(queue, channel);
        string messageId = Guid.NewGuid().ToString();

        var props = channel.CreateBasicProperties();
        props.ContentType = Program.MESSAGE_CONTENT_TYPE;
        props.ContentEncoding = Program.MESSAGE_ENCODING;
        props.MessageId = messageId;
        props.DeliveryMode = 2;

        byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(_currentTypeExchange, queue, props, messageBodyBytes);
        return messageId;
    }

    public override string Call(IModel channel, int userId, List<string> subscribes = null, List<string> unsubscribes = null, string topic = "", string message = "")
    {
        if (topic == string.Empty || message == string.Empty)
            throw new Exception("Wrong topic or msg.");

        _currentTypeExchange = Program.USER_TOPICS_EXCHANGE;
        _topic = topic;

        string messageId = Guid.NewGuid().ToString();

        var props = channel.CreateBasicProperties();
        props.ContentEncoding = Program.MESSAGE_ENCODING;
        props.ContentType = Program.MESSAGE_CONTENT_TYPE;
        props.MessageId = messageId;
        props.DeliveryMode = 2;

        byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(_currentTypeExchange, _topic, props, messageBodyBytes);
        return messageId;
    }

    public override string GetDescriptioin()
        => $"Sending message to user: {_userId}; topic: {_topic}";

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
        channel.QueueBind(queue, _currentTypeExchange, routingKey);
    }
}
