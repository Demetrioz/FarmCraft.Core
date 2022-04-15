namespace FarmCraft.Core.Data.Entities
{
    /// <summary>
    /// The base of all FarmCraft tables, containing fields
    /// for the date created, date modified, and
    /// a soft delete
    /// </summary>
    public class FarmCraftBase
    {
        /// <summary>
        /// The date a row / entity was created
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// The date a row / entity was last modified
        /// </summary>
        public DateTimeOffset Modified { get; set; }

        /// <summary>
        /// Whether the row / entity is soft-deleted
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
