using FarmCraft.Core.Data.Entities;
using FarmCraft.Core.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FarmCraft.Core.Controllers
{
    /// <summary>
    /// A base controller that should be inherited by all 
    /// FarmCraft service controllers. It contains shared
    /// logic such as paginating results, etc.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public abstract class FarmCraftController : ControllerBase
    {
        public FarmCraftController() { }

        /// <summary>
        /// Returns a paginated reusult, given the items that should
        /// be returned, and the requested page number / size
        /// </summary>
        /// <typeparam name="T">Any class implementing the FarmCraftBase class</typeparam>
        /// <param name="items">The items that should be returned</param>
        /// <param name="pageNumber">The page number</param>
        /// <param name="pageSize">How many items are returned with each page</param>
        /// <returns></returns>
        protected PaginatedResult<T> ToPaginatedResult<T>(
            IQueryable<T> items,
            int pageNumber,
            int pageSize
        )
            where T : FarmCraftBase
        {
            string scheme = HttpContext.Request.Scheme;
            QueryString queryString = HttpContext.Request.QueryString;
            HostString host = HttpContext.Request.Host;
            PathString path = HttpContext.Request.Path;
            var requestUrl = $"{scheme}://{host}{path}{queryString}";

            return PaginatedResult<T>
                .ToPaginatedResult(items, pageNumber, pageSize, requestUrl);
        }
    }
}
