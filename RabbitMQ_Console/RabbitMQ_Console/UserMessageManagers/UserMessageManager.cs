#nullable disable

using RabbitMQ_Console.ChannelCallable;

namespace RabbitMQ_Console.UserMessageManagers;

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
        _rabbitMqManager.Call_WithTopics(new DeclareOkChannelCallable(), -1);
    }

    internal void OnUserLogin(int userId)
    {
        _rabbitMqManager.Call(new BindOkChannelCallable(), userId);
    }

    internal string SendUserMessage(int userId, string message)
    {
        return _rabbitMqManager.Call(new SendUserMessageChannelCallable(), userId, message);
    }

    internal string SendUserMessage(int userId, string message, string topic)
    {
        return _rabbitMqManager.Call_WithTopics(new SendUserMessageChannelCallable(), userId, null, null, topic, message);
    }

    internal List<string> FetchUserMessages(int userId, string message)
    {
        return _rabbitMqManager.Call(new FetchUserMessagesChannelCallable(), userId, message);
    }

    internal void OnUserTopicInterestChange(int userId, List<string> subscribes, List<string> unsubscribes)
    {
        _rabbitMqManager.Call_WithTopics(new OnUserTopicInterestChange_ChannelCallable(), userId, subscribes, unsubscribes, string.Empty, string.Empty);
    }
}
