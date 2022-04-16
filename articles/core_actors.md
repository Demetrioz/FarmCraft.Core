# The Core Actor System

The core actor system is where all of the business logic takes place for a given
FarmCraft microservice. Doing it this way allows us to trigger the logic when
receiving messages from both a message broker and an api request without having
to duplicate logic. It also allows us to run the logic in container separate
from the database, api, and any other service by using Akka.Remote.

In this section, we'll work our way from the bottom (an actual message), up to
the very top (the core system).

## Create a FarmCraft Message

First, let's look at creating a message.

```c#
public interface IDemoMessage { }

public class DemoMessageOne : FarmCraftMessage, IDemoMessage { }
```

We create an interface first, because the interface will be used by the
FarmCraftManager to route messages to the appropriate FarmCraft actor. This
basically means that any message that inherits from IDemoMessage will be routed
to the DemoActor. The DemoActor is then responsible for handling that message
appropriately.

All Messages in FarmCraft services extend the FarmCraftMessage. This is a base
message that contains a

- Timestamp (DateTimeOffset)
- MessageType (Type)
- Data (object)

Most times, you won't have to implement anything past this.

## Create a FarmCraft Actor

The FarmCraft Actor is responsible for receiving a set of messages (the
interface from above), and handling them with the correct functions.

```c#
public class DemoActor : FarmCraftActor
{
  public TestActor(IServiceProvider provider) : base(provider)
  {
    Receive<DemoMessage>(message => HandleDemoMessage(message));
  }

  private void HandleDemoMessage(DemoMessage message)
  {
    // Logic goes here
  }
}
```

Here, we're saying that any time we receive a DemoMessage, the logic from
HandleDemoMessage should run. If we have other services that are required, we
can simply grab them from the provider within the handler.

```c#
private void HandleDemoMessage(DemoMessage message)
{
  using (IServiceScope scope = _serviceProvider.CreateScope())
  using (ISomeService myService = scope.ServiceProvider.GetService<ISomeService>())
  {
    // Logic goes here
  }
}
```

## Create a FarmCraftManager

The FarmCraftManager is our root actor in the service's Actor System. All
requests, whether from the API or Message Broker will go to the manager first.
The manager is responsible for routing messages to the correct child actors.

```c#
public class DemoManager : FarmCraftManager
{
  public DemoManager(IServiceProvider provider) : base(provider)
  {
    Receive<FarmCraftMessage>(message => HandleMessage(message));
  }

  public void HandleMessage(FarmCraftMessage message)
  {
    switch(message.MessageType)
    {
      case IDemoMessage demo:
        HandleWithInstanceOf<DemoActor>(demo);
        break;
      default:
        break;
    }
  }
}
```

FarmCraftManagers will only Receive a FarmCraftMessage, as all messages
originating from FarmCraft are FarmCraftMessages. When handling those messages;
however, we can use the interface from above to route them to the correct place.
In this case, any message that inherits from IDemoMessage will be routed to
the DemoActor.

HandleWithInstanceOf is a built-in function that creates an actor on demand,
allows that actor to run its logic, and then destroys the actor after it's
finished.

If you need a particular actor to be long-lived, you can create that within the
constructor and reference it inside of HandleMessage

```c#
public class DemoManager : FarmCraftManager
{
  private readonly IActorRef _myLongLivedActor;

  public DemoManager(IServiceProvider provider) : base(provider)
  {
    _myLogLivedActor = Context.ActorOf(
      Props.Create(() => new MyLogLivedActor()),
      "MyActorName"
    );
  }

  public void HandleMessage(FarmCraftMessage message)
  {
    switch(message.MessageType)
    {
      case ISomeMessage m:
        _myLongLivedActor.Ask(m);
        break;
      default:
        break;
    }
  }
}
```

## Create a FarmCraftCore

A FarmCraftCore is the main background service that runs everything. It contains
the ActorSystem, an IServiceProvider, and Root Actor.

First, we extend FarmCraftCore in our own implementation.

```c#
public class FarmCraftDemo : FarmCraftCore<DemoManager>
{
  public FarmCraftDemo(IServiceProvider provider) : base(provider) { }
}
```

If we would like our service to listen to a message bus for incoming messages,
we can register the connection settings in our startup.

```c#
.ConfigureServices(services =>
{
  ...

  services.Configure<ConsumerOptions>(options =>
  {
    options.Host = "Connection String / Host Name";
    options.Queue = "Queue Name";
  });

  ...
})
```

Then, we create an IMessageConsumer of our choice, and register a handler to
run when receiving messages.

```c#
public class FarmCraftDemo : FarmCraftCore<DemoManager>
{
  private readonly IMessageConsumer _messageConsumer;

  public FarmCraftDemo(
    IServiceProvider provider,
    IOptions<ConsumerOptions> options,
    ILogger<FarmCraftDemo> logger
  ) : base(provider)
  {
    _messageConsumer = new ServiceBusConsumer(options, logger);
    _messageConsumer.Register<DemoMessageOne>((message) => HandleMessage(message));
  }

  private void HandleMessage(FarmCraftMessage message)
  {
    _root.Tell(message);
  }
}
```

What we're doing here is creating a consumer for an Azure Service Bus. Then
we register the HandleMessage function as the "handler" for any message that
we receive of type DemoMessageOne. If we had other messages that we would
be receiving from the service bus, we would just add them with the following:

```c#
_messageConsumer.Register<MyOtherMessage>((message) => HandleMessage(message));
```

## Configuration

How to configure

- actor.conf
- environment variables
