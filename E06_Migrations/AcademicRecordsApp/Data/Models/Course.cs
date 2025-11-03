namespace AcademicRecordsApp.Data.Models
{
    public class Course
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public virtual ICollection<Student> Students { get; set; }
            = new HashSet<Student>();

        public virtual ICollection<Exam> Exams { get; set; }
            = new HashSet<Exam>();
    }
}
