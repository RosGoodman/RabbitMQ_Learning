#nullable disable

using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ_Console.ChannelCallable;

namespace RabbitMQ_Console;

/// <summary>
/// RabbitMqManager.
/// </summary>
/// <remarks>
/// Код адаптирован из книги RabbitMQ Essentials - David Dossot.
/// </remarks>
internal class RabbitMqManager
{
    private static ILogger _logger;
    private ConnectionFactory _factory;
    private IConnection _connection;

    internal RabbitMqManager(ILogger logger, ConnectionFactory factory)
    {
        _logger = logger;
        _factory = factory;
        _connection = null;
    }

    internal void Start()
    {
        var endpoints = new List<AmqpTcpEndpoint>
        {
            new AmqpTcpEndpoint("hostname"),
            new AmqpTcpEndpoint("localhost")
        };

        try
        {
            _connection = _factory.CreateConnection(endpoints);
            _logger.Log(LogLevel.Information, $"Connected to host: {_factory.HostName} port: {_factory.Port}");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, $"Connected to host: {_factory.HostName} port: {_factory.Port} >>> {ex.Message}");
        }
    }

    internal void Stop()
    {
        if (_connection is null) return;

        try
        {
            _connection.Close();
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, $"Connected to host: {_factory.HostName} port: {_factory.Port} >>> {ex.Message}");
        }
        finally
        {
            _connection = null;
        }
    }

    /// <summary>
    /// Create and returne fresh channel.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Though channel instances are technically thread safe, it is
    /// strongly recommended that you avoid having several threads
    /// using the same channel concurrently.
    /// </remarks>
    internal IModel CreateChannel()
    {
        try
        {
            return _connection is null ? null : _connection.CreateModel();
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, $"Failed to create channel >>> {ex.Message}");
            return null;
        }
    }

    internal void CloseChannel(IModel channel)
    {
        if(channel is null || !channel.IsOpen) 
            return;

        try
        {
            channel.Close();
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, $"Failed to close channel >>> {ex.Message}");
        }
    }

    internal T Call<T>(ChannelCallableAbstract<T> callable, int userId = -1, string message = "")
        where T : class
    {
        var channel = CreateChannel();

        if(channel is null) return null;

        try
        {
            return callable.Call(channel, userId, message);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, $"Failed to run: {callable.GetDescriptioin()} on channel: {channel} >>> {ex.Message}");
        }
        finally
        {
            CloseChannel(channel);
        }

        return null;
    }

    internal T Call_WithTopics<T>(ChannelCallableAbstract<T> callable, int userId, List<string> subscribes = null, List<string> unsubscribes = null, string topic = "", string message = "")
        where T : class
    {
        var channel = CreateChannel();

        if(channel is null) return null;

        try
        {
            return callable.Call(channel, userId, subscribes, unsubscribes, topic, message);
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, $"Failed to run: {callable.GetDescriptioin()} on channel: {channel} >>> {ex.Message}");
        }
        finally
        {
            CloseChannel(channel);
        }

        return null;
    }
}
