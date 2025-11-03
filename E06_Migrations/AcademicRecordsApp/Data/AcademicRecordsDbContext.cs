namespace AcademicRecordsApp.Data
{
    using Microsoft.EntityFrameworkCore;

    using Models;

    public partial class AcademicRecordsDbContext : DbContext
    {
        public AcademicRecordsDbContext()
        {

        }

        public AcademicRecordsDbContext(DbContextOptions<AcademicRecordsDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<Exam> Exams { get; set; } = null!;

        public virtual DbSet<Grade> Grades { get; set; } = null!;

        public virtual DbSet<Student> Students { get; set; } = null!;

        public virtual DbSet<Course> Courses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.GetConnectionString());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Exam>(entity =>
            {
                entity
                    .HasKey(e => e.Id)
                    .HasName("PK__Exams__3214EC078D932BAA");

                entity
                    .Property(e => e.Name)
                    .HasMaxLength(100);

                entity
                    .HasOne(e => e.Course)
                    .WithMany(c => c.Exams)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Grade>(entity =>
            {
                entity
                    .HasKey(e => e.Id)
                    .HasName("PK__Grades__3214EC07811E1AE1");

                entity
                    .Property(e => e.Value)
                    .HasColumnType("decimal(3, 2)");

                entity
                    .HasOne(d => d.Exam)
                    .WithMany(p => p.Grades)
                    .HasForeignKey(d => d.ExamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Grades_Exams");

                entity
                    .HasOne(d => d.Student)
                    .WithMany(p => p.Grades)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Grades_Students");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity
                    .HasKey(e => e.Id);

                entity
                    .Property(e => e.Title)
                    .HasMaxLength(100);

                entity
                    .Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsRequired(false);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity
                    .HasKey(e => e.Id)
                    .HasName("PK__Students__3214EC07AB437044");

                entity
                    .Property(e => e.FullName)
                    .HasMaxLength(100);

                entity
                    .HasMany(s => s.Courses)
                    .WithMany(c => c.Students)
                    .UsingEntity<Dictionary<string, object>>(
                        "StudentsCourses",
                        r => r
                            .HasOne<Course>()
                            .WithMany()
                            .HasForeignKey("CourseId")
                            .OnDelete(DeleteBehavior.ClientSetNull),
                        l => l
                            .HasOne<Student>()
                            .WithMany()
                            .HasForeignKey("StudentId")
                            .OnDelete(DeleteBehavior.ClientSetNull),
                        me =>
                        {
                            me.HasKey("StudentId", "CourseId");
                            me.ToTable("StudentsCourses");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}