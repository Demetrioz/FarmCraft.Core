using FarmCraft.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FarmCraft.Core.Data.Context
{
    /// <summary>
    /// FarmCraftContext is an interface that can be inherited by the DbContext of all
    /// FarmCraft Services and contains a common log DbSet the logic for initializing triggers on the base 
    /// columns of Created, Updated, and IsDeleted
    /// </summary>
    public interface IFarmCraftContext
    {
        DbSet<FarmCraftLog> Logs { get; set; }
        public ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default);
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
