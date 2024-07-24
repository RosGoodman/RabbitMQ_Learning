#nullable disable

using RabbitMQ_Console.ChannelCallable;
using System.Text;

namespace RabbitMQ_Console;

public class UserMessageManager
{
    private RabbitMqManager _rabbitMqManager;

    internal UserMessageManager(RabbitMqManager rabbitMqManager)
    {
        _rabbitMqManager = rabbitMqManager;
    }

    internal void OnApplicationStart()
    {
        _rabbitMqManager.Call(new DeclareOkChannelCallable());
    }

    internal void OnUserLogin(int userId)
    {
        _rabbitMqManager.Call(new BindOkChannelCallable(), userId);
    }

    internal string SendUserMessage(int userId, string message)
    {
        return _rabbitMqManager.Call(new SendUserMessageChannelCallable(), userId, message);
    }

    internal List<string> FetchUserMessages(int userId, string message)
    {
        return _rabbitMqManager.Call(new FetchUserMessagesChannelCallable(), userId, message);
    }
}
