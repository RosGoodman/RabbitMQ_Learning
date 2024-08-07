﻿#nullable disable

using Microsoft.Extensions.Logging;
using RabbitMQ_Console.UserMessageManagers;

namespace RabbitMQ_Console;

public class UserSimulator
{
    private static List<string> _topics = new() { "science", "politics", "sports", "fashion" };

    private readonly ILogger _logger;
    private int _userId, _maxUserId;
    private UserMessageManager _userMessageManager;

    public UserSimulator(ILogger logger, int userId, int maxUserId, UserMessageManager userMessageManager)
    {
        _logger = logger;
        _userId = userId;
        _maxUserId = maxUserId;
        _userMessageManager = userMessageManager;
    }

    public void Run()
    {
        _userMessageManager.OnUserLogin(_userId);
        _userMessageManager.OnUserTopicInterestChange(_userId, new() { "science", "politics" }, new());

        _logger.Log(LogLevel.Information, $"User login: {_userId}");

        var rnd = new Random();

        while (Thread.CurrentThread.IsAlive)
        {
            try
            {
                //получение сообщений
                var messages = _userMessageManager.FetchUserMessages(_userId, string.Empty);
                if (messages is not null)
                {
                    foreach (var message in messages)
                        _logger.Log(LogLevel.Information, $"User {_userId} recived: {message}");
                }

                //отправка сообщений
                if (rnd.Next(1, 200) < 25)
                {
                    int addresseeUserId = 1 + rnd.Next(_maxUserId);
                    if (addresseeUserId != _userId)
                    {
                        string jsonMessage = $"{addresseeUserId} Greatings! Hello from: {_userId}. Timestamp: {DateTime.Now}";
                        _userMessageManager.SendUserMessage(_userId, jsonMessage);
                    }
                }

                //отправка сообщений с топиками
                if (rnd.Next(1, 400) < 5)
                {
                    string topic = _topics[rnd.Next(0, 1)];

                    string jsonMessage = $"New content about {topic}. Published by {_userId}. Timestamp: {DateTime.Now}";
                    _logger.Log(LogLevel.Information, $"User {_userId} Published new content.");
                    _userMessageManager.SendUserMessage(_userId, jsonMessage, topic);
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"{ex.Message}");
            }

            try
            {
                Thread.Sleep(1000);
            }
            catch (Exception)
            {
                Thread.CurrentThread.Interrupt();
            }
        }
    }
}
