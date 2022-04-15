namespace FarmCraft.Core.Queries
{
    /// <summary>
    /// Query Parameters that are received by FarmCraft API endpoints
    /// </summary>
    internal class QueryParameters
    {
        /// <summary>
        /// The largest number of results returned for a single request
        /// </summary>
        private const int _maxPageSize = 100;

        /// <summary>
        /// The default number of results returned for a single request
        /// </summary>
        private int _pageSize = 50;

        /// <summary>
        /// The current page of results
        /// </summary>
        private int _page = 1;

        /// <summary>
        /// The text that is being searched for
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// The table fields or columns that should be searched for
        /// the SearchText
        /// </summary>
        public string SearchFields { get; set; }

        /// <summary>
        /// The beginning time we should start searching from
        /// </summary>
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// The end time we should stop searching from
        /// </summary>
        public DateTimeOffset? StopTime { get; set; }

        /// <summary>
        /// A public function to expose _page
        /// </summary>
        public int Page
        {
            get { return _page; }
            set
            {
                if (value < 1)
                    _page = 1;
                else
                    _page = value;
            }
        }

        /// <summary>
        /// A public function to expose _pageSize
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (value > _maxPageSize)
                    _pageSize = _maxPageSize;
                else
                    _pageSize = value;
            }
        }
    }
}
