using Akka.Actor;

namespace FarmCraft.CoreTest
{
    public class TestActor : ReceiveActor
    {
        public TestActor()
        {
            Receive<string>(message => HandleMessage(message));
        }

        private void HandleMessage(string message)
        {
            var pause = true;
        }
    }
}
