using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SecTester.Bus.Dispatchers;

public interface RmqConnectionManager : IDisposable
{
  bool IsConnected { get; }

  void Connect();

  void TryConnect();

  IModel CreateChannel();

  AsyncEventingBasicConsumer CreateConsumer(IModel channel);
}
