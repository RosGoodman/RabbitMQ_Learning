using Microsoft.Extensions.Logging;
using RabbitMQ_Console.UserMessageManagers;

namespace RabbitMQ_Console;

/// <remarks>
/// Код адаптирован из книги RabbitMQ Essentials - David Dossot.
/// </remarks>
internal class Program
{
    internal const string USER_INBOXES_EXCHANGE = "user-inboxes";
    internal const string USER_TOPICS_EXCHANGE = "user-topics";
    internal const string MESSAGE_CONTENT_TYPE = "application/vnd.ccm.pmsg.v1+json";
    internal const string MESSAGE_ENCODING = "UTF-8";

    static void Main(string[] args)
    {
        ILogger logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        }).CreateLogger<Program>();

        var rabbitMqManager = new RabbitMqManager(logger, Connection.GetConnectionFactory());

        rabbitMqManager.Start();

        var messageManager = new UserMessageManager(rabbitMqManager);
        messageManager.OnApplicationStart();

        int maxUserId = 12;
        logger.Log(LogLevel.Information, $"Starting the application with simulated users {maxUserId}");

        var threads = new List<Thread>();
        for ( int i = 0; i < maxUserId; i++)
        {
            var thread = new Thread(new UserSimulator(logger, i, maxUserId, messageManager).Run);
            threads.Add(thread);
            thread.Start();
            Thread.Sleep(1500);
        }

        foreach ( var thread in threads)
        {
            thread.Interrupt();
            thread.Join();
        }


        logger.Log(LogLevel.Information, "Shutting down...");

        //var channel = rabbitMqManager.CreateChannel();
        //rabbitMqManager.CloseChannel(channel);
        rabbitMqManager.Stop();

        logger.Log(LogLevel.Information, "Bye!");
    }
}
