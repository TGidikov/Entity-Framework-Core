using P01_StudentSystem.Data.Models;
using Microsoft.EntityFrameworkCore;


namespace P01_StudentSystem.Data
{
   public class StudentSystemContext:DbContext
   {
        public StudentSystemContext()
        {

        }

        public StudentSystemContext(DbContextOptions options)
            :base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);

            }
            base.OnConfiguring(optionsBuilder);

        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<Homework> HomeworkSubmissions { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.StudentId);

                entity
                .Property(s => s.Name)
                .IsUnicode()
                .IsRequired()
                .HasMaxLength(100);

                entity
                .Property(s => s.PhoneNumber)
                .IsRequired(false)
                .IsUnicode(false)
                .HasMaxLength(10);

                entity
                .Property(s => s.Birthday)
                .IsRequired(false);

            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.CourseId);

                entity
                .Property(c => c.Name)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(80);

                entity
                .Property(c => c.Description)
                .IsRequired(false)
                .IsUnicode();



            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(r => r.ResourceId);

                entity
                .Property(r => r.Name)
                .IsRequired()
                .IsUnicode()
                .HasMaxLength(50);

                entity
                .Property(r => r.Url)
                .IsUnicode(false);

                entity
                .HasOne(c => c.Course)
                .WithMany(c => c.Resources)
                .HasForeignKey(c => c.CourseId);
                

            });

            modelBuilder.Entity<Homework>(entity =>
            {
                entity.HasKey(h => h.HomeworkId);

                entity
                .Property(h => h.Content)
                .IsRequired()
                .IsUnicode(false);

                entity
                .HasOne(h => h.Student)
                .WithMany(h => h.HomeworkSubmissions)
                .HasForeignKey(h => h.StudentId);

                entity
                .HasOne(h => h.Course)
                .WithMany(h => h.HomeworkSubmissions)
                .HasForeignKey(h => h.CourseId);


            });
            modelBuilder.Entity<StudentCourse>(entity =>
            {
                entity.HasKey(e => new { e.StudentId, e.CourseId });

                entity.HasOne(e => e.Student)
                .WithMany(e => e.CourseEnrollments)
                .HasForeignKey(e => e.StudentId);

                entity.HasOne(e => e.Course)
                .WithMany(e => e.StudentsEnrolled)
                .HasForeignKey(e => e.CourseId);



            });






        }
    }
}
