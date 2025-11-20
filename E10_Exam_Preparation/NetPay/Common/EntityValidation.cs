namespace NetPay.Common
{
    public static class EntityValidation
    {
        public static class Household
        {
            public const int ContactPersonMinLength = 5;
            public const int ContactPersonMaxLength = 50;
            public const int EmailMinLength = 6;
            public const int EmailMaxLength = 80;
            public const int PhoneNumberLength = 15;
            public const string PhoneNumberRegExPattern = @"^\+\d{3}/\d{3}-\d{6}$";
        }

        public static class Expense
        {
            public const int ExpenseNameMinLength = 5;
            public const int ExpenseNameMaxLength = 50;
            public const string AmountMinValue = "0.01";
            public const string AmountMaxValue = "100000.0";
            public const string AmountSqlType = "DECIMAL(8,2)";
        }

        public static class Service
        {
            public const int ServiceNameMinLength = 5;
            public const int ServiceNameMaxLength = 30;
        }

        public static class Supplier
        {
            public const int SupplierNameMinLength = 3;
            public const int SupplierNameMaxLength = 60;
        }
    }
}
