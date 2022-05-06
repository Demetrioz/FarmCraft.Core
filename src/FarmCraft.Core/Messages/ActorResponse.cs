using FarmCraft.Core.Data.DTOs;

namespace FarmCraft.Core.Messages
{
    /// <summary>
    /// A helper class to return various types of responses
    /// from FarmCraft Actors
    /// </summary>
    public static class ActorResponse
    {
        public static FarmCraftActorResponse Success(
            string requestId, 
            object data
        ) =>
            new FarmCraftActorResponse
            {
                RequestId = Guid.Parse(requestId),
                Status = ResponseStatus.Success,
                Data = data,
                Error = null
            };

        public static FarmCraftActorResponse Failure(
            string requestId,
            string message
        ) =>
            new FarmCraftActorResponse
            {
                RequestId = Guid.Parse(requestId),
                Status = ResponseStatus.Failure,
                Data = null,
                Error = message
            };
    }
}
