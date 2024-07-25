#nullable disable

using RabbitMQ.Client;

namespace RabbitMQ_Console.ChannelCallable;

internal class OnUserTopicInterestChange_ChannelCallable : ChannelCallableAbstract<object>
{
    private string _currentTypeExchange = string.Empty;
    private int _userId = -1;
    private List<string> _subscribes = new();
    private List<string> _unsubscribes = new();

    public override object Call(IModel channel, int userId = -1, string message = "")
    {
        throw new NotImplementedException();
    }

    public override object Call(IModel channel, int userId, List<string> subscribes, List<string> unsubscribes, string topic = "", string message = "")
    {
        if (userId == -1)
            return null;

        _userId = userId;

        subscribes ??= new();
        unsubscribes ??= new();

        _currentTypeExchange = Program.USER_TOPICS_EXCHANGE;
        string queue = GetUserInboxQueue();

        for (int i = 0; i < subscribes.Count; i++)
            channel.QueueBind(queue, _currentTypeExchange, subscribes[i]);

        for (int i = 0; i < unsubscribes.Count; i++)
            channel.QueueUnbind(queue, _currentTypeExchange, unsubscribes[i]);

        return null;
    }

    public override string GetDescriptioin()
        => $"Binding user queue: {GetUserInboxQueue()} to exchange: {_currentTypeExchange}.";
    private string GetUserInboxQueue()
        => $"user-inbox.{_userId}";
}
