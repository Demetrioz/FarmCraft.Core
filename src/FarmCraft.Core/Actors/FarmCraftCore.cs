using Akka.Actor;
using Akka.Configuration;
using Akka.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FarmCraft.Core.Actors
{
    /// <summary>
    /// The background service that makes up any FarmCraft service. It contains the 
    /// actor system, the root actor, and a message consumer to receive messages from
    /// a message broker
    /// </summary>
    /// <typeparam name="T">Any class that derives from the FarmCraftManager type</typeparam>
    public class FarmCraftCore<T> : BackgroundService where T : FarmCraftManager
    {
        private ActorSystem _actorSystem;
        private IActorRef _root;
        private readonly IServiceProvider _serviceProvider;

        public FarmCraftCore(IServiceProvider provider)
        {
            _serviceProvider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var hocon = ConfigurationFactory.ParseString(await File.ReadAllTextAsync("actor.conf", stoppingToken));
            var bootStrap = BootstrapSetup.Create().WithConfig(hocon);
            var di = DependencyResolverSetup.Create(_serviceProvider);
            var actorSystemSetup = bootStrap.And(di);
            _actorSystem = ActorSystem.Create("FarmCraftCore", actorSystemSetup);
            _root = _actorSystem.ActorOf(Props.Create(() => (T)Activator.CreateInstance(typeof(T))), "FarmCraftManager");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await CoordinatedShutdown.Get(_actorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
        }
    }
}
