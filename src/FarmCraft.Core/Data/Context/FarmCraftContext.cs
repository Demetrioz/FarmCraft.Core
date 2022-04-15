using EntityFrameworkCore.Triggers;
using FarmCraft.Core.Data.Entities;
using FarmCraft.Core.Data.Triggers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FarmCraft.Core.Data.Context
{
    /// <summary>
    /// FarmCraftContext is a base DbContext that should be inherited by the DbContext of all
    /// FarmCraft Services. It contains the logic for initializing triggers on the base 
    /// columns of Created, Updated, and IsDeleted
    /// </summary>
    public abstract class FarmCraftContext : DbContextWithTriggers
    {
        public FarmCraftContext(DbContextOptions<FarmCraftContext> options) : base(options) { }

        /// <summary>
        /// Static method called during startup to initialize triggers on the database
        /// </summary>
        public static void SetTriggers()
        {
            SetBaseTriggers();
        }

        /// <summary>
        /// Iterates through all tables in the database, getting a list of those
        /// that extend FarmCraftBase, to apply the DefaultTriggers
        /// </summary>
        protected static void SetBaseTriggers()
        {
            List<Type> dbSets = typeof(FarmCraftContext)
                .GetProperties()
                .Where(p => p.PropertyType.Name == "DbSet`1")
                .Select(p => p.PropertyType.GetGenericArguments().FirstOrDefault())
                .ToList();

            List<Type> baseTypes = typeof(FarmCraftContext)
                .Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(FarmCraftBase)) && t.IsClass)
                .ToList();

            List<Type> baseSets = baseTypes
                .Where(t => dbSets.Contains(t))
                .ToList();

            MethodInfo defaultTriggersMethod = typeof(FarmCraftContext)
                .GetMethod(nameof(SetDefaultEntityTriggers));

            foreach (Type t in baseSets)
            {
                MethodInfo genericTriggerMethod = defaultTriggersMethod.MakeGenericMethod(t);
                genericTriggerMethod.Invoke(null, null);
            }
        }

        /// <summary>
        /// Sets insert and update triggers on the Created, Updated, and IsDeleted
        /// columns of a given entity, T
        /// </summary>
        /// <typeparam name="T">Any entity that inherits the FarmCraftBase class</typeparam>
        public static void SetDefaultEntityTriggers<T>() where T : FarmCraftBase
        {
            Triggers<T>.Inserting += BaseTriggers<T>.OnInserting;
            Triggers<T>.Updating += BaseTriggers<T>.OnUpdating;
        }
    }
}
