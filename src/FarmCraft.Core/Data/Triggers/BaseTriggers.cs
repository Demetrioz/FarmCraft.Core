using EntityFrameworkCore.Triggers;
using FarmCraft.Core.Data.Entities;

namespace FarmCraft.Core.Data.Triggers
{
    /// <summary>
    /// Base triggers for FarmCraft Contexts to update the Created,
    /// Modified, and IsDeleted Fields
    /// </summary>
    /// <typeparam name="T">Any entity that inherits the FarmCraftBase class</typeparam>
    public static class BaseTriggers<T> where T : FarmCraftBase
    {
        /// <summary>
        /// When inserting a row, sets the Created and Modified fields to the 
        /// current UTC time and IsDeleted to false
        /// </summary>
        /// <param name="entry">A FarmCraftBase entity from the DbContext</param>
        public static void OnInserting(IInsertingEntry entry)
        {
            FarmCraftBase entity = entry.Entity as FarmCraftBase;
            if (entity != null)
            {
                DateTimeOffset now = DateTimeOffset.UtcNow;

                SetCreated(entity, now);
                SetModified(entity, now);
                SetIsDeleted(entity);
            }
        }

        /// <summary>
        /// When updating a row, sets the Modified field to the current
        /// UTC time
        /// </summary>
        /// <param name="entry">A FarmCraftBase entity from the DbContext</param>
        public static void OnUpdating(IUpdatingEntry entry)
        {
            FarmCraftBase entity = entry.Entity as FarmCraftBase;
            if (entity != null)
            {
                DateTimeOffset now = DateTimeOffset.UtcNow;

                SetModified(entity, now);
            }
        }

        /// <summary>
        /// Sets the Created column of a FarmCraftBase entity to the given time
        /// </summary>
        /// <param name="entity">A FarmCraftBase entity from the DbContext</param>
        /// <param name="now">The current UTC time</param>
        public static void SetCreated(FarmCraftBase entity, DateTimeOffset now)
        {
            entity.Created = now;
        }

        /// <summary>
        /// Sets the Modified column of a FarmCraftBase entity to the given time
        /// </summary>
        /// <param name="entity">A FarmCraftBase entity from the DbContext</param>
        /// <param name="now">The current UTC time</param>
        public static void SetModified(FarmCraftBase entity, DateTimeOffset now)
        {
            entity.Modified = now;
        }

        /// <summary>
        /// Sets the IsDeleted column of a FarmCraftBase entity to false
        /// </summary>
        /// <param name="entity">A FarmCraftBase entity from the DbContext</param>
        public static void SetIsDeleted(FarmCraftBase entity)
        {
            entity.IsDeleted = false;
        }
    }
}
