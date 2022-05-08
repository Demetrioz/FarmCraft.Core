﻿using Akka.Actor;
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
    public abstract class FarmCraftCore<T> : BackgroundService where T : FarmCraftManager
    {
        protected ActorSystem _actorSystem;
        protected IActorRef _root;
        protected readonly IServiceProvider _serviceProvider;

        public FarmCraftCore(IServiceProvider provider)
        {
            _serviceProvider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var hocon = ConfigurationFactory.Default();
            var bootStrap = BootstrapSetup.Create().WithConfig(hocon);
            var di = DependencyResolverSetup.Create(_serviceProvider);
            var actorSystemSetup = bootStrap.And(di);
            _actorSystem = ActorSystem.Create("FarmCraftCore", actorSystemSetup);
            InitializeRootActor();
        }

        protected virtual void InitializeRootActor()
        {
            var props = DependencyResolver.For(_actorSystem).Props<T>();
            _root = _actorSystem.ActorOf(props, "FarmCraftManager");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await CoordinatedShutdown.Get(_actorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
        }
    }
}
