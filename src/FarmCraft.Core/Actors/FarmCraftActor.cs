using Akka.Actor;

namespace FarmCraft.Core.Actors
{
    /// <summary>
    /// A base Actor for the FarmCraft system. Contains the service provider
    /// for access to additional service via dependency injection
    /// </summary>
    public abstract class FarmCraftActor : ReceiveActor
    {
        protected readonly IServiceProvider _serviceProvider;

        public FarmCraftActor(IServiceProvider provider)
        {
            _serviceProvider = provider;
        }
    }
}
