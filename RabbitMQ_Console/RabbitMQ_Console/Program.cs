﻿using Microsoft.Extensions.Logging;

namespace RabbitMQ_Console;

/// <remarks>
/// Код адаптирован из книги RabbitMQ Essentials - David Dossot.
/// </remarks>
internal class Program
{
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