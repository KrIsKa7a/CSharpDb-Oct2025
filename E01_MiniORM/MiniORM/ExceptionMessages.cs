namespace MiniORM
{
    public static class ExceptionMessages
    {
        public const string NullEntityAddedMessage = "Entity cannot be null.";

        public const string PopulateDbSetNotFoundMessage =
            "There was an internal error while populating the DbSet. Please make sure that your AppDbContext inherits from the MiniORM DbContext.";

        public const string NullDbSetMessage =
            "There was an internal error while populating the DbSet.";

        public const string MapRelationsNotFoundMessage =
            "There was an internal error while mapping relations. Please make sure that your AppDbContext inherits from the MiniORM DbContext.";

        public const string MapNavigationCollectionNotFoundMessage =
            "There was an internal error while mapping navigation collections. Please make sure that your AppDbContext inherits from the MiniORM DbContext.";

        public static string InvalidEntitiesInDbSetMessage =
            @"{0} Invalid Entities Found in {1}!";

        public static string TransactionRollbackMessage =
            @"Performing Rollback due to Exception!!!";

        public static string TransactionExceptionMessage =
            @"The SQL Transaction failed due to unexpected error!";
    }
}
