#nullable disable

using RabbitMQ.Client;

namespace RabbitMQ_Console.ChannelCallable;

public class DeclareOkChannelCallable : ChannelCallableAbstract<object>
{
    private string _currentTypeExchange = string.Empty;

    public override object Call(IModel channel, int userId = -1, string message = "")
    {
        _currentTypeExchange = Program.USER_INBOXES_EXCHANGE;

        string t = "direct";
        //survive a server restart
        bool durable = true;
        //keep it even if nobody is using it
        bool autoDelete = false;
        //no special arguments
        Dictionary<string, object> arguments = new();

        channel.ExchangeDeclare(_currentTypeExchange, t, durable, autoDelete, arguments);
        return null;
    }

    public override object Call(IModel channel, int userId = -1, List<string> subscribes = null, List<string> unsubscribes = null, string topic = "", string message = "")
    {
        _currentTypeExchange = Program.USER_TOPICS_EXCHANGE;
        string t = "topic";
        //survive a server restart
        bool durable = true;
        //keep it even if nobody is using it
        bool autoDelete = false;
        //no special arguments
        Dictionary<string, object> arguments = new();

        channel.ExchangeDeclare(_currentTypeExchange, t, durable, autoDelete, arguments);
        return null;
    }

    public override string GetDescriptioin()
        => $"Declaring direct exchange: {_currentTypeExchange}";
}
