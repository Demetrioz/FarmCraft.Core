using FarmCraft.Core.Data.Entities;

namespace FarmCraft.Core.Messages.Telemetry
{
    public class AskToPublishTelemetry
    {
        public FarmCraftTelemetry Telemetry { get; private set; }

        public AskToPublishTelemetry(FarmCraftTelemetry telemetry)
        {
            Telemetry = telemetry;
        }
    }
}
