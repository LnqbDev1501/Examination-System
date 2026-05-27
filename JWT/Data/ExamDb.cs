using JWT.Models;
using Microsoft.EntityFrameworkCore;

namespace JWT.Data
{
    public class ExamDb: DbContext
    {
        public ExamDb(DbContextOptions<ExamDb> options) : base(options)
        {
        }
        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Exam> Exams { get; set; }
        public DbSet<Models.Question> Questions { get; set; }
        public DbSet<Models.QuestionOption> QuestionOptions { get; set; }
        public DbSet<Models.ExamAttempt> ExamAttempts { get; set; }
        public DbSet<Models.AttemptQuestion> AttemptQuestions { get; set; }
        public DbSet<Models.StudentAnswer> StudentAnswers { get; set; }
        public DbSet<Models.StudentAnswerOption> StudentAnswerOptions { get; set; }
        public DbSet<Models.Notification> Notifications { get; set; }
        public DbSet<TeacherSubject> TeacherSubjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.SubjectId })
                .IsUnique();

            modelBuilder.Entity<ExamQuestion>()
                .HasIndex(eq => new { eq.ExamId, eq.QuestionId })
                .IsUnique();

            modelBuilder.Entity<StudentAnswer>()
                .HasIndex(sa => new { sa.AttemptId, sa.QuestionId })
                .IsUnique();

            // TeacherRequest có 2 FK về User
            modelBuilder.Entity<TeacherRequest>()
                .HasOne(tr => tr.Student)
                .WithMany(u => u.SentTeacherRequests)
                .HasForeignKey(tr => tr.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TeacherRequest>()
                .HasOne(tr => tr.Reviewer)
                .WithMany(u => u.ReviewedTeacherRequests)
                .HasForeignKey(tr => tr.ReviewedBy)
                .OnDelete(DeleteBehavior.NoAction);

            // Fix multiple cascade paths
            modelBuilder.Entity<ExamAttempt>()
                .HasOne(ea => ea.Student)
                .WithMany(u => u.ExamAttempts)
                .HasForeignKey(ea => ea.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExamAttempt>()
                .HasOne(ea => ea.Exam)
                .WithMany(e => e.ExamAttempts)
                .HasForeignKey(ea => ea.ExamId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Teacher)
                .WithMany(u => u.Exams)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Subject)
                .WithMany(s => s.Exams)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Teacher)
                .WithMany(u => u.Questions)
                .HasForeignKey(q => q.TeacherId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Subject)
                .WithMany(s => s.Questions)
                .HasForeignKey(q => q.SubjectId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Exam)
                .WithMany(e => e.ExamQuestions)
                .HasForeignKey(eq => eq.ExamId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Question)
                .WithMany(q => q.ExamQuestions)
                .HasForeignKey(eq => eq.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AttemptQuestion>()
                .HasOne(aq => aq.ExamAttempt)
                .WithMany(ea => ea.AttemptQuestions)
                .HasForeignKey(aq => aq.AttemptId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AttemptQuestion>()
                .HasOne(aq => aq.Question)
                .WithMany(q => q.AttemptQuestions)
                .HasForeignKey(aq => aq.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.ExamAttempt)
                .WithMany(ea => ea.StudentAnswers)
                .HasForeignKey(sa => sa.AttemptId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.Question)
                .WithMany(q => q.StudentAnswers)
                .HasForeignKey(sa => sa.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentAnswerOption>()
                .HasOne(sao => sao.StudentAnswer)
                .WithMany(sa => sa.StudentAnswerOptions)
                .HasForeignKey(sao => sao.StudentAnswerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentAnswerOption>()
                .HasOne(sao => sao.QuestionOption)
                .WithMany(qo => qo.StudentAnswerOptions)
                .HasForeignKey(sao => sao.OptionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<QuestionOption>()
                .HasOne(qo => qo.Question)
                .WithMany(q => q.QuestionOptions)
                .HasForeignKey(qo => qo.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Subject)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Decimal
            modelBuilder.Entity<Question>()
                .Property(q => q.Score)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Exam>()
                .Property(e => e.TotalScore)
                .HasPrecision(6, 2);

            modelBuilder.Entity<Exam>()
                .Property(e => e.PassingScore)
                .HasPrecision(6, 2);

            modelBuilder.Entity<ExamQuestion>()
                .Property(eq => eq.Score)
                .HasPrecision(5, 2);

            modelBuilder.Entity<ExamAttempt>()
                .Property(ea => ea.Score)
                .HasPrecision(6, 2);

            modelBuilder.Entity<AttemptQuestion>()
                .Property(aq => aq.Score)
                .HasPrecision(5, 2);

            modelBuilder.Entity<StudentAnswer>()
                .Property(sa => sa.ScoreAwarded)
                .HasPrecision(5, 2);

            // RowVersion
            modelBuilder.Entity<User>()
                .Property(u => u.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<Subject>()
                .Property(s => s.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<TeacherRequest>()
                .Property(t => t.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<Question>()
                .Property(q => q.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<QuestionOption>()
                .Property(qo => qo.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<Exam>()
                .Property(e => e.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<ExamAttempt>()
                .Property(ea => ea.RowVersion)
                .IsRowVersion();

            // Seed roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "Teacher" },
                new Role { RoleId = 3, RoleName = "Student" }
            );

        }

    }
}
