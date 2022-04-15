using Akka.Actor;

namespace FarmCraft.Core.Actors
{
    /// <summary>
    /// The root actor of the FarmCraft services. This class can be implemented differently
    /// within each service, but will manage the overall actors and message routing.
    /// </summary>
    public abstract class FarmCraftManager : FarmCraftActor
    {
        public FarmCraftManager(IServiceProvider provider) : base(provider)
        {
        }

        /// <summary>
        /// Handles messages by creating an instance of an actor of the given type. This is
        /// useful in situations where you have a one off message that doesn't require a
        /// long-running actor. After processing the message, the actor is destroyed.
        /// </summary>
        /// <typeparam name="T">Any class deriving from the FarmCraftActor type</typeparam>
        /// <param name="message">The message object that should be handled by the
        /// created actor</param>
        /// <returns>Returns the actor's response to the caller</returns>
        protected async Task HandleWithInstanceOf<T>(object message) where T : FarmCraftActor
        {
            // Since we're async, we lose the context if we don't make a reference to it
            IUntypedActorContext context = Context;
            IActorRef sender = Sender;

            string actorName = $"{typeof(T).Name}-{DateTimeOffset.Now:yyyy-MM-ddHH:mm:ss:fffffff}";
            IActorRef actorRef = context.ActorOf(
                Props.Create(() => (T)Activator.CreateInstance(typeof(T), _serviceProvider)),
                actorName
            );

            object result = await actorRef.Ask(message, TimeSpan.FromSeconds(30));

            // Since we're creating a temporary instance, make sure we kill it when finished
            actorRef.Tell(PoisonPill.Instance);

            // Return the result to whoever asked for it
            sender.Tell(result);
        }
    }
}
