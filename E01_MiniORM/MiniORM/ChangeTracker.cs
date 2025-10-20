namespace MiniORM
{
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;

    public class ChangeTracker<TEntity>
        where TEntity : class, new()
    {
        private readonly ICollection<TEntity> allEntities;
        private readonly ICollection<TEntity> added;
        private readonly ICollection<TEntity> removed;

        public ChangeTracker(IEnumerable<TEntity> allEntities)
        {
            this.added = new List<TEntity>();
            this.removed = new List<TEntity>();
            this.allEntities = CloneEntities(allEntities);
        }

        /// <summary>
        /// Keeps track of the existing entities in the database.
        /// </summary>
        public IReadOnlyCollection<TEntity> AllEntities => 
            (IReadOnlyCollection<TEntity>)this.allEntities;

        /// <summary>
        /// Keeps track of the added entities which are not yet persisted in the database.
        /// </summary>
        public IReadOnlyCollection<TEntity> Added =>
            (IReadOnlyCollection<TEntity>)this.added;

        /// <summary>
        /// Keeps track of the removed entities which are not yet persisted in the database.
        /// </summary>
        public IReadOnlyCollection<TEntity> Removed =>
            (IReadOnlyCollection<TEntity>)this.removed;

        /// <summary>
        /// Marks the given entity as added.
        /// </summary>
        /// <param name="entity">Entity to be inserted.</param>
        public void Add(TEntity entity)
        {
            this.added.Add(entity);
        }

        /// <summary>
        /// Marks the given entity as removed.
        /// </summary>
        /// <param name="entity">Entity to be deleted.</param>
        public void Remove(TEntity entity)
        {
            this.removed.Add(entity);
        }

        public IEnumerable<TEntity> GetModifiedEntities(DbSet<TEntity> dbSet)
        {
            ICollection<TEntity> modifiedEntities = new List<TEntity>();

            // Usually will be array with single property inside
            // But may be array of several properties in case of composite PK
            PropertyInfo[] primaryKeys = typeof(TEntity)
                .GetProperties()
                .Where(pi => pi.HasAttribute<KeyAttribute>())
                .ToArray();

            foreach (TEntity proxyEntity in this.AllEntities)
            {
                IEnumerable<object> primaryKeyValues = GetPrimaryKeyValues(proxyEntity, primaryKeys);
                TEntity localEntity = dbSet.Entities
                    .Single(le => GetPrimaryKeyValues(le, primaryKeys).SequenceEqual(primaryKeyValues));

                bool isModified = IsModified(proxyEntity, localEntity);
                if (isModified)
                {
                    modifiedEntities.Add(localEntity);
                }
            }

            return modifiedEntities;
        }

        /// <summary>
        /// Performs shallow copy of a collection of reference type.
        /// </summary>
        /// <param name="entities">Collection to be cloned</param>
        /// <returns>Cloned collection.</returns>
        private static ICollection<TEntity> CloneEntities(IEnumerable<TEntity> entities)
        {
            ICollection<TEntity> clonedEntities = new List<TEntity>();
            
            PropertyInfo[] propertiesToClone = typeof(TEntity)
                .GetProperties()
                .Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType))
                .ToArray();
            foreach (TEntity entity in entities)
            {
                TEntity clonedEntity = Activator
                    .CreateInstance<TEntity>();
                foreach (PropertyInfo property in propertiesToClone)
                {
                    object? propertyVal = property.GetValue(entity);
                    property.SetValue(clonedEntity, propertyVal);
                }
            }

            return clonedEntities;
        }

        private static IEnumerable<object> GetPrimaryKeyValues(TEntity entity, PropertyInfo[] primaryKeys)
        {
            return primaryKeys
                .Select(pk => pk.GetValue(entity))!;
        }

        private static bool IsModified(TEntity proxyEntity, TEntity localEntity)
        {
            PropertyInfo[] trackedProperties = typeof(TEntity)
                .GetProperties()
                .Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType))
                .ToArray();

            return trackedProperties
                .Any(pi => !Equals(pi.GetValue(proxyEntity), pi.GetValue(localEntity)));
        }
    }
}