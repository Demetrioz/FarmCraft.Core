using Akka.Actor;
using FarmCraft.Core.Actors;

namespace FarmCraft.CoreTest
{
    public class TestCore : FarmCraftCore<TestManager>
    {
        public TestCore(IServiceProvider provider) : base(provider)
        {
            var pause = true;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await base.ExecuteAsync(stoppingToken);
            _root.Tell("HelloWorld");
        }
    }
}