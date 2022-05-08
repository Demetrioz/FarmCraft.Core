using Akka.Actor;
using Akka.DependencyInjection;

namespace FarmCraft.Core.Actors
{
    /// <summary>
    /// The root actor of the FarmCraft services. This class can be implemented differently
    /// within each service, but will manage the overall actors and message routing.
    /// </summary>
    public abstract class FarmCraftManager : ReceiveActor
    {
        public FarmCraftManager()
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
        protected async Task HandleWithInstanceOf<T>(object message) where T : ReceiveActor
        {
            // Since we're async, we lose the context if we don't make a reference to it
            IUntypedActorContext context = Context;
            IActorRef sender = Sender;

            string actorName = $"{typeof(T).Name}-{DateTimeOffset.Now:yyyy-MM-ddHH:mm:ss:fffffff}";
            Props props = DependencyResolver.For(context.System).Props<T>();
            IActorRef actorRef = context.ActorOf(props, actorName);

            object result = await actorRef.Ask(message, TimeSpan.FromSeconds(30));

            // Since we're creating a temporary instance, make sure we kill it when finished
            actorRef.Tell(PoisonPill.Instance);

            // Return the result to whoever asked for it
            sender.Tell(result);
        }
    }
}
