using FarmCraft.Core.Data.Entities;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace FarmCraft.Core.Queries
{
    /// <summary>
    /// A class representing a paginated result, returned from an 
    /// API request
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedResult<T> where T : FarmCraftBase
    {
        /// <summary>
        /// The current page of results
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// The total number of pages for the given search
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// How many items contained in each page
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// The total number of items for the given search
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// The Url containing the next page of results, if
        /// there is one
        /// </summary>
        public string NextPageUrl { get; set; }

        /// <summary>
        /// The Url containing the previous page of results, 
        /// if there is one
        /// </summary>
        public string PreviousPageUrl { get; set; }

        /// <summary>
        /// A list of the actual search items contained within this
        /// page of results
        /// </summary>
        public IEnumerable<T> Items { get; private set; }

        /// <summary>
        /// Whether a page of results has a previous page
        /// </summary>
        private bool HasPreviousPage
        {
            get { return CurrentPage > 1; }
        }

        /// <summary>
        /// Whether a page of results has a next page
        /// </summary>
        private bool HasNextPage
        {
            get { return CurrentPage < TotalPages; }
        }

        /// <summary>
        /// Constructs paginated reuslt object, given the parameters
        /// </summary>
        /// <param name="items">A list of items that should be returned </param>
        /// <param name="itemCount">The number of items being returned</param>
        /// <param name="pageNumber">The page number that we're returning</param>
        /// <param name="itemsPerPage">How many items are returned per page</param>
        /// <param name="requestUrl">The URL used to generate the response</param>
        public PaginatedResult(
            IEnumerable<T> items,
            int itemCount,
            int pageNumber,
            int itemsPerPage,
            string requestUrl
        )
        {
            Items = items;
            TotalItems = itemCount;
            ItemsPerPage = itemsPerPage;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(itemCount / (double)itemsPerPage);

            if (CurrentPage > 1)
                PreviousPageUrl = GenerateRequestUrl(requestUrl);

            if (HasNextPage)
                NextPageUrl = GenerateRequestUrl(requestUrl, true);
        }

        /// <summary>
        /// Constructs paginated reuslt object, given the parameters
        /// </summary>
        /// <param name="items">A list of items that should be returned </param>
        /// <param name="pageNumber">The page number that we're returning</param>
        /// <param name="itemsPerPage">How many items are returned per page</param>
        /// <param name="requestUrl">The URL used to generate the response</param>
        /// <returns></returns>
        public static PaginatedResult<T> ToPaginatedResult(
            IQueryable<T> items,
            int pageNumber,
            int itemsPerPage,
            string requestUrl
        )
        {
            int totalItems = items.Count();
            IEnumerable<T> pageItems = items
                .Skip((pageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage);

            return new PaginatedResult<T>(
                pageItems,
                totalItems,
                pageNumber,
                itemsPerPage,
                requestUrl
            );
        }

        /// <summary>
        /// Generates a previous or next url object, given the 
        /// current url and whether there should be a next page
        /// </summary>
        /// <param name="currentUrl">The URL used to make the current request</param>
        /// <param name="next">Whether a next page exists</param>
        /// <returns></returns>
        private string GenerateRequestUrl(string currentUrl, bool next = false)
        {
            string[] urlParts = currentUrl.Split("?");
            string url = urlParts[0];
            string queryString = urlParts.Count() > 1 ? urlParts[1] : "";

            Dictionary<string, StringValues> queryParams = QueryHelpers.ParseQuery(queryString);
            if (!queryParams.TryGetValue("page", out StringValues page))
                queryParams.Add("page", CurrentPage.ToString());

            if (!next && HasPreviousPage)
            {
                int.TryParse(queryParams["page"], out int currentPage);
                --currentPage;

                if (currentPage > TotalPages)
                    currentPage = TotalPages;

                queryParams["page"] = currentPage.ToString();
                return QueryHelpers.AddQueryString(url, (IDictionary<string, string>)queryParams);
            }
            else if (next && HasNextPage)
            {
                int.TryParse(queryParams["page"], out int currentPage);
                ++currentPage;

                if (currentPage <= 1)
                    currentPage = CurrentPage + 1;

                queryParams["page"] = currentPage.ToString();
                return QueryHelpers.AddQueryString(url, (IDictionary<string, string>)queryParams);
            }
            else
                return null;
        }
    }
}
