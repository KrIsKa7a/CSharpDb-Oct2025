namespace MiniORM
{
    using System.Collections;

    public class DbSet<TEntity> : ICollection<TEntity>
        where TEntity : class, new()
    {
        internal DbSet(IEnumerable<TEntity> entities)
        {
            this.Entities = entities.ToList();
            this.ChangeTracker = new ChangeTracker<TEntity>(entities);
        }

        public int Count
            => this.Entities.Count;

        public bool IsReadOnly
            => this.Entities.IsReadOnly;

        public ChangeTracker<TEntity> ChangeTracker { get; }

        internal ICollection<TEntity> Entities { get; }

        public void Add(TEntity? item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item),
                    ExceptionMessages.NullEntityAddedMessage);
            }

            this.Entities.Add(item);
            this.ChangeTracker.Add(item);
        }

        public bool Remove(TEntity? item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item),
                    ExceptionMessages.NullEntityAddedMessage);
            }

            bool removed = this.Entities.Remove(item);
            if (removed)
            {
                this.ChangeTracker.Remove(item);
            }

            return removed;
        }

        public bool RemoveRange(IEnumerable<TEntity> entitiesToRemove)
        {
            bool result = true;
            if (entitiesToRemove == null)
            {
                throw new ArgumentNullException(nameof(entitiesToRemove),
                    ExceptionMessages.NullEntityAddedMessage);
            }

            foreach (TEntity entity in entitiesToRemove)
            {
                result &= this.Remove(entity);
            }

            return result;
        }

        public void Clear()
        {
            while (this.Entities.Any())
            {
                this.Remove(this.Entities.First());
            }
        }

        public bool Contains(TEntity item)
        {
            return this.Entities.Contains(item);
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            this.Entities.CopyTo(array, arrayIndex);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return this.Entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
