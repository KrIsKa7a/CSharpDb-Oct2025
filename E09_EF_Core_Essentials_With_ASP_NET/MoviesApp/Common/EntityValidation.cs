namespace MoviesApp.Common
{
    public static class EntityValidation
    {
        public const int MovieTitleMinLength = 2;
        public const int MovieTitleMaxLength = 150;
        public const int MovieGenreMinLength = 3;
        public const int MovieGenreMaxLength = 50;
        public const int MovieDirectorMinLength = 3;
        public const int MovieDirectorMaxLength = 100;
        public const int MovieDescriptionMinLength = 20;
        public const int MovieDescriptionMaxLength = 1000;
        public const int MovieImageUrlMaxLength = 2048;
        public const int MovieDurationMinValue = 1;
        public const int MovieDurationMaxValue = 600;
        public const int MovieDurationTextMaxLength = 3;
        public const double MovieRatingMinValue = 0.0;
        public const double MovieRatingMaxValue = 10.0;
        public const int MovieRatingSourceMaxLength = 50;
        public const string MovieReleaseDateRegExprPattern = @"^(\d{4})\-(\d{2})\-(\d{2})$";
    }
}
