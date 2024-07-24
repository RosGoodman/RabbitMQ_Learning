#nullable disable

using RabbitMQ.Client;

namespace RabbitMQ_Console.ChannelCallable;

public class DeclareOkChannelCallable : ChannelCallableAbstract<object>
{
    private static string USER_INBOXES_EXCHANGE = "user-inboxes";

    public override object Call(IModel channel, int userId = -1, string message = "")
    {
        string t = "direct";
        //survive a server restart
        bool durable = true;
        //keep it even if nobody is using it
        bool autoDelete = false;
        //no special arguments
        Dictionary <string, object> arguments = new();

        channel.ExchangeDeclare(USER_INBOXES_EXCHANGE, t, durable, autoDelete, arguments);
        return null;
    }

    public override string GetDescriptioin() 
        => $"Declaring direct exchange: {USER_INBOXES_EXCHANGE}";
}
