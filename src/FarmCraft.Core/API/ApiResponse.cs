using Microsoft.AspNetCore.Mvc;

namespace FarmCraft.Core.API
{
    /// <summary>
    /// A helper class to return various types of responses from 
    /// FarmCraft API endpoints
    /// </summary>
    public static class ApiResponse
    {
        /// <summary>
        /// Returns a successful response object
        /// </summary>
        /// <param name="data">The data returned from the API</param>
        /// <returns>An OkObjectResult</returns>
        public static IActionResult Success(object data) =>
            new OkObjectResult(new FarmCraftApiResponse
            {
                RequestId = Guid.NewGuid(),
                Data = data,
                Error = null
            });

        /// <summary>
        /// Returns a created response object
        /// </summary>
        /// <param name="data">The data returned from the API</param>
        /// <returns>A CreatedResult</returns>
        public static IActionResult Created(object data) =>
            new CreatedResult("created", new FarmCraftApiResponse
            {
                RequestId = Guid.NewGuid(),
                Data = data,
                Error = null
            });

        /// <summary>
        /// Return sa conflict response object
        /// </summary>
        /// <param name="error">The error message from the API</param>
        /// <returns>A ConflictObjectResult</returns>
        public static IActionResult Conflict(string error) =>
            new ConflictObjectResult(new FarmCraftApiResponse
            {
                RequestId = Guid.NewGuid(),
                Data = null,
                Error = error
            });

        /// <summary>
        /// Returns an unauthorized response object
        /// </summary>
        /// <returns>An UnauthorizedObjectResult</returns>
        public static IActionResult Unauthorized() =>
            new UnauthorizedObjectResult(new FarmCraftApiResponse
            {
                RequestId = Guid.NewGuid(),
                Data = null,
                Error = "Unauthorized"
            });

        /// <summary>
        /// Returns a bad request response object
        /// </summary>
        /// <param name="error">The error message from the API</param>
        /// <returns>A BadRequestObjectResult</returns>
        public static IActionResult BadRequest(string error) =>
            new BadRequestObjectResult(new FarmCraftApiResponse
            {
                RequestId = Guid.NewGuid(),
                Data = null,
                Error = error
            });
    }
}
