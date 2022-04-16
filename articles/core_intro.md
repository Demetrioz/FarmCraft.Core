# How to use FarmCraft.Core

FarmCraft.Core provides the base functionality for FarmCraft microservices. In
order to utilize this functionality, you need to extend the core classes and
add your own functionality on top of them.

FarmCraft makes use of message brokers, [Akka](https://getakka.net/), and REST
APIs, so if you're not familiar with those topics, I recommend starting there
before continuing.

For this example, let's assume we're going to create a microservice called
FarmCraft.Demo.

Here are the main areas that we need to cover:

1. The core actor system
2. The API
3. Database connections
4. Building the service
5. Utilizing a message broker
