#nullable disable

using RabbitMQ.Client;
using System.Text;

namespace RabbitMQ_Console.ChannelCallable;

internal class FetchUserMessagesChannelCallable : ChannelCallableAbstract<List<string>>
{
    private static long _userId = -1;

    public override List<string> Call(IModel channel, int userId = -1, string message = "")
    {
        _userId = userId;
        var messages = new List<string>();

        var queue = GetUserInboxQueue(userId);
        var autoAck = true;

        BasicGetResult getResponse;
        Encoding enc;

        while ((getResponse = channel.BasicGet(queue, autoAck)) is not null)
        {
            enc = Encoding.GetEncoding(getResponse.BasicProperties.ContentEncoding);
            messages.Add(enc.GetString(getResponse.Body.ToArray()));
        }

        return messages;
    }

    public override string GetDescriptioin()
        => $"Fetching messages for user: {_userId}";
    private string GetUserInboxQueue(int userId)
        => $"user-inbox.{userId}";
}
