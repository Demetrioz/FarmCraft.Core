using FarmCraft.Core.Actors;
using FarmCraft.CoreTest.Config;
using Microsoft.Extensions.Options;

namespace FarmCraft.CoreTest
{
    public class TestManager : FarmCraftManager
    {
        private readonly TestSettings settings;

        public TestManager(IOptions<TestSettings> options)
        {
            settings = options.Value;

            Receive<string>(message => HandleMessage(message));

            HandleWithInstanceOf<TestActor>("HelloWorld");
        }

        private void HandleMessage(string message)
        {
            var pause = true;
        }
    }
}
