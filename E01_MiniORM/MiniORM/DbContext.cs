namespace MiniORM
{
    using System.Collections;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;

    using Microsoft.Data.SqlClient;

    using static ExceptionMessages;

    public abstract class DbContext
    {
        private readonly DatabaseConnection connection;
        private readonly IDictionary<Type, PropertyInfo> dbSetProperties;

        protected DbContext(string connectionString)
        {
            this.connection = new DatabaseConnection(connectionString);
            this.dbSetProperties = this.DiscoverDbSets();

            using (new ConnectionManager(this.connection))
            {
                this.InitializeDbSets();
            }

            this.MapAllRelations();
        }

        internal static ICollection<Type> AllowedSqlTypes = new HashSet<Type>()
        {
            typeof(string),
            typeof(char),
            typeof(char?),
            typeof(bool),
            typeof(bool?),
            typeof(byte),
            typeof(byte?),
            typeof(short),
            typeof(short?),
            typeof(int),
            typeof(int?),
            typeof(long),
            typeof(long?),
            typeof(float),
            typeof(float?),
            typeof(double),
            typeof(double?),
            typeof(decimal),
            typeof(decimal?),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(TimeSpan),
            typeof(TimeSpan?),
            typeof(DateOnly),
            typeof(DateOnly?),
            typeof(TimeOnly),
            typeof(TimeOnly?)
        };

        public void SaveChanges()
        {
            IEnumerable<object> dbSetsObjects = this.dbSetProperties
                .Select(edb => edb.Value.GetValue(this)!)
                .ToArray();
            foreach (IEnumerable<object> dbSet in dbSetsObjects)
            {
                IEnumerable<object> invalidEntities = dbSet
                    .Where(e => !IsObjectValid(e))
                    .ToArray();
                if (invalidEntities.Any())
                {
                    throw new InvalidOperationException(String.Format(InvalidEntitiesInDbSetMessage,
                        invalidEntities.Count(), dbSet.GetType().Name));
                }
            }

            using (new ConnectionManager(this.connection))
            {
                using SqlTransaction transaction = this.connection
                    .StartTransaction();
                foreach (IEnumerable dbSet in dbSetsObjects)
                {
                    MethodInfo? persistMethod = typeof(DbContext)
                        .GetMethod("Persist", BindingFlags.NonPublic | BindingFlags.Instance)?
                        .MakeGenericMethod(dbSet.GetType());

                    try
                    {
                        try
                        {
                            persistMethod?.Invoke(this, new object[] { dbSet });
                        }
                        catch (TargetInvocationException tie)
                            when (tie.InnerException != null)
                        {
                            throw tie.InnerException;
                        }
                    }
                    catch
                    {
                        Console.WriteLine(TransactionRollbackMessage);
                        transaction.Rollback();
                        throw;
                    }

                }

                try
                {
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(TransactionExceptionMessage);
                    throw;
                }
            }
        }

        private void Persist<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            string tableName = this.GetTableName(typeof(TEntity));
            IEnumerable<string> columnNames = this.connection
                .FetchColumnNames(tableName);

            if (dbSet.ChangeTracker.Added.Any())
            {
                this.connection
                    .InsertEntities(dbSet.ChangeTracker.Added, tableName, columnNames.ToArray());
            }

            IEnumerable<TEntity> modifiedEntities = dbSet
                .ChangeTracker
                .GetModifiedEntities(dbSet);
            if (modifiedEntities.Any())
            {
                this.connection
                    .UpdateEntities(modifiedEntities, tableName, columnNames.ToArray());
            }

            if (dbSet.ChangeTracker.Removed.Any())
            {
                this.connection
                    .DeleteEntities(dbSet.ChangeTracker.Removed, tableName, columnNames.ToArray());
            }
        }

        private IDictionary<Type, PropertyInfo> DiscoverDbSets()
        {
            return this.GetType()
                .GetProperties()
                .Where(pi => pi.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .ToDictionary(pi => pi.PropertyType.GetGenericArguments().First(), pi => pi);
        }

        private void InitializeDbSets()
        {
            foreach (KeyValuePair<Type, PropertyInfo> dbSetPropertyPair in this.dbSetProperties)
            {
                Type entityType = dbSetPropertyPair.Key;
                PropertyInfo dbSetProperty = dbSetPropertyPair.Value;

                MethodInfo? populateDbSetMethodGeneric = typeof(DbContext)
                    .GetMethod(nameof(PopulateDbSet), BindingFlags.Instance | BindingFlags.NonPublic)?
                    .MakeGenericMethod(entityType);
                if (populateDbSetMethodGeneric == null)
                {
                    throw new InvalidOperationException(ExceptionMessages.PopulateDbSetNotFoundMessage);
                }

                populateDbSetMethodGeneric.Invoke(this, new object[] { dbSetProperty });
            }
        }

        private void PopulateDbSet<TEntity>(PropertyInfo dbSetPropertyInfo)
            where TEntity : class, new()
        {
            IEnumerable<TEntity> tableEntities = this.LoadTableEntities<TEntity>();
            DbSet<TEntity> dbSetInstance = new DbSet<TEntity>(tableEntities);

            ReflectionHelper.ReplaceBackingField(this, dbSetPropertyInfo.Name, dbSetInstance);
        }

        private IEnumerable<TEntity> LoadTableEntities<TEntity>()
            where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);
            string tableName = this.GetTableName(entityType);
            string[] columnNames = this.GetEntityColumnNames(entityType).ToArray();

            IEnumerable<TEntity> tableEntities = this.connection
                .FetchResultSet<TEntity>(tableName, columnNames);
            return tableEntities;
        }

        private IEnumerable<string> GetEntityColumnNames(Type entityType)
        {
            string tableName = this.GetTableName(entityType);
            IEnumerable<string> columnNames = this.connection
                .FetchColumnNames(tableName);

            IEnumerable<string> entityColumnNames = entityType
                .GetProperties()
                .Where(pi => columnNames.Contains(pi.Name, StringComparer.InvariantCultureIgnoreCase) &&
                             pi.HasAttribute<NotMappedAttribute>() == false &&
                             AllowedSqlTypes.Contains(pi.PropertyType))
                .Select(pi => pi.Name)
                .ToArray();

            return entityColumnNames;
        }

        private string GetTableName(Type entityType)
        {
            string? tableName = entityType.GetCustomAttribute<TableAttribute>()?.Name;
            if (tableName == null)
            {
                tableName = this.dbSetProperties[entityType].Name;
            }

            return tableName;
        }

        private void MapAllRelations()
        {
            foreach (KeyValuePair<Type, PropertyInfo> dbSetPropertyPair in this.dbSetProperties)
            {
                Type entityType = dbSetPropertyPair.Key;
                object? dbSetInstance = dbSetPropertyPair.Value.GetValue(this);
                if (dbSetInstance == null)
                {
                    throw new InvalidOperationException(ExceptionMessages.NullDbSetMessage);
                }

                MethodInfo? mapRelationsMethodGeneric = typeof(DbContext)
                    .GetMethod(nameof(MapRelations), BindingFlags.Instance | BindingFlags.NonPublic)?
                    .MakeGenericMethod(entityType);
                if (mapRelationsMethodGeneric == null)
                {
                    throw new InvalidOperationException(ExceptionMessages.MapRelationsNotFoundMessage);
                }

                mapRelationsMethodGeneric.Invoke(this, new object[] { dbSetInstance });
            }
        }

        private void MapRelations<TEntity>(DbSet<TEntity> dbSetInstance)
            where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);
            this.MapNavigationProperties(dbSetInstance);

            PropertyInfo[] navigationCollectionsProperties = entityType
                .GetProperties()
                .Where(pi => pi.PropertyType.IsGenericType &&
                                        pi.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>) &&
                                        this.dbSetProperties.ContainsKey(pi.PropertyType.GetGenericArguments().First()))
                .ToArray();
            foreach (PropertyInfo navigationCollectionPropInfo in navigationCollectionsProperties)
            {
                Type collectionEntityType = navigationCollectionPropInfo.PropertyType.GetGenericArguments().First();
                MethodInfo? mapCollectionMethodGeneric = typeof(DbContext)
                    .GetMethod(nameof(MapNavigationCollection), BindingFlags.Instance | BindingFlags.NonPublic)?
                    .MakeGenericMethod(entityType, collectionEntityType);
                if (mapCollectionMethodGeneric == null)
                {
                    throw new InvalidOperationException(ExceptionMessages.MapNavigationCollectionNotFoundMessage);
                }

                mapCollectionMethodGeneric.Invoke(this, new object[] { dbSetInstance, navigationCollectionPropInfo });
            }
        }

        private void MapNavigationProperties<TEntity>(DbSet<TEntity> dbSetInstance)
            where TEntity : class, new()
        {
            Type entityType = typeof(TEntity);
            PropertyInfo[] foreignKeyProperties = entityType
                .GetProperties()
                .Where(pi => pi.HasAttribute<ForeignKeyAttribute>())
                .ToArray();
            foreach (PropertyInfo foreignKeyProperty in foreignKeyProperties)
            {
                string navigationPropertyName = foreignKeyProperty
                    .GetCustomAttribute<ForeignKeyAttribute>()!
                    .Name;
                PropertyInfo navigationProperty = entityType
                    .GetProperties()
                    .First(pi => pi.Name == navigationPropertyName);

                object? navigationDbSetInstance = this.dbSetProperties[navigationProperty.PropertyType]
                    .GetValue(this);
                if (navigationDbSetInstance == null)
                {
                    throw new InvalidOperationException(ExceptionMessages.NullDbSetMessage);
                }

                PropertyInfo navigationPrimaryKey = navigationProperty
                    .PropertyType
                    .GetProperties()
                    .First(pi => pi.HasAttribute<KeyAttribute>());
                foreach (TEntity entity in dbSetInstance)
                {
                    object? foreignKeyValue = foreignKeyProperty.GetValue(entity);
                    if (foreignKeyValue == null)
                    {
                        continue;
                    }

                    object navigationEntity = ((IEnumerable<object>)navigationDbSetInstance)
                        .First(ne => navigationPrimaryKey.GetValue(ne)!.Equals(foreignKeyValue));
                    navigationProperty.SetValue(entity, navigationEntity);
                }
            }
        }
        
        private void MapNavigationCollection<TEntity, TCollectionEntity>(DbSet<TEntity> dbSetInstance,
            PropertyInfo navigationCollectionPropertyInfo)
            where TEntity : class, new()
            where TCollectionEntity : class, new()
        {
            Type entityType = typeof(TEntity);
            Type collectionEntityType = typeof(TCollectionEntity);

            PropertyInfo primaryKey = entityType
                .GetProperties()
                .First(pi => pi.HasAttribute<KeyAttribute>());
            PropertyInfo foreignKey = collectionEntityType
                .GetProperties()
                .First(pi => pi.HasAttribute<ForeignKeyAttribute>() &&
                             collectionEntityType
                                 .GetProperty(pi.GetCustomAttribute<ForeignKeyAttribute>()!.Name)!
                                 .PropertyType == entityType);

            DbSet<TCollectionEntity>? navCollectionDbSet =
                (DbSet<TCollectionEntity>?)this.dbSetProperties[collectionEntityType].GetValue(this);
            if (navCollectionDbSet == null)
            {
                throw new InvalidOperationException(ExceptionMessages.NullDbSetMessage);
            }

            foreach (TEntity entity in dbSetInstance)
            {
                object? entityPrimaryKeyValue = primaryKey.GetValue(entity);
                ICollection<TCollectionEntity> navigationEntities = navCollectionDbSet
                    .Where(ne => foreignKey.GetValue(ne)?.Equals(entityPrimaryKeyValue) ?? false)
                    .ToArray();
                ReflectionHelper.ReplaceBackingField(entity, navigationCollectionPropertyInfo.Name, navigationEntities);
            }
        }

        private static bool IsObjectValid(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, validationResults);
        }
    }
}
