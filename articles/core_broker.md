# Working with the Message Broker

FarmCraft services are tied together through a message broker for communication.
Those services aren't the only ones who can utilize the broker however, and
other applications can send messages to the FarmCraftServices.

## Sending A Message

Before you can send a message to one of the FarmCraft microservices, you first
need to create a message that it accepts. To do this, you can use the
microservice's FarmCraft.SERVICENAME.Data package.

After that, the simplest way to send a message is to use an IMessagePublisher.
Assuming you have an Azure Service Bus, utilizing an IMessagePublisher may look
similar to this:

```c#
PublisherOptions pubOptions = new PublisherOptions
{
  Host = "ConnectionString",
  Queue = "QueueName"
};

var options = Options.Create(pubOptions);

ServiceBusPublisher publisher = new ServiceBusPublisher(options, logger);
```

From here, any time you need to publish a message, you simply call
Publisher.PublishMessage()

```c#
private async Task MyFunc()
{
  await publisher.PublishMessage(new MyMessage
  {
    Timestamp = DateTimeOffset.UtcNow,
    MessageType = typeof(MyMessage),
    Data = new MyData(myParams)
  });
}
```

## Subscribe to messages

FarmCraft.Core has a simple way to subscribe to queues on a message broker via
IMessageConsumer. Again, utilizing the Azure Service Bus might look like this:

```c#
ConsumerOptions conOptions = new ConsumerOptions
{
  Host = "ConnectionString",
  Queue = "QueueName"
};

var options = Options.Create(conOptions);

ServiceBusConsumer consumer = new ServiceBusConsumer(options, logger);
```

At this point, we have a consumer listening to "QueueName", but it can't do
anything with the messages it receives. To change that, we have two options.

### Register a Handler Class

The first option is to register a handler class. To do this, we create a class
that inherits from FarmCraftMessageHandler.

```c#
public class MyHandler : FarmCraftMessageHandler<MyData>
{
  public override Task HandleMessage(MyData data)
  {
    // Logic goes here
  }
}
```

The FarmCraftMessageHandler takes care of parsing the data from the received
message, and sends it to a HandleMessage function that we define.

Once the class has been created, we can register it with the consumer by calling
the register function.

```c#
consumer.Register<MyMessage, MyHandler>();
```

### Register a Handler Action

The second ay we can handle messages inside a consumer is to create an action
for a particular message type.

```c#
private void HandleMessage(FarmCraftMessage message)
{
  // Logic goes here
}
```

Once the handler's been defined, you can register it as follows:

```c#
consumer.Register<MyMessage>((message) => HandleMessage(message));
```
