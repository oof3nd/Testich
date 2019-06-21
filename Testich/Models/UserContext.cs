using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Testich.Models
{
    public class UserContext : IdentityDbContext<User>
    {
        public DbSet<Test> Tests { get; set; }
        public DbSet<Category> Сategories { get; set; }
        public DbSet<ResultScale> ResultScales { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagTest> TagTest { get; set; }
        public DbSet<LevelOfTest> LevelsOfTest { get; set; }
        public DbSet<TypeOfQuestion> TypeOfQuestions { get; set; }
        public DbSet<QuestionOfTest> QuestionOfTests { get; set; }
        public DbSet<ClosedQuestion> ClosedQuestions { get; set; }
        public DbSet<ClosedQuestionOption> ClosedQuestionOptions { get; set; }
        public DbSet<Result> Results { get; set; }

        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
           Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<TagTest>()
                .HasKey(t => new { t.TagId, t.TestId });

            modelBuilder.Entity<TagTest>()
                .HasOne(sc => sc.Tag)
                .WithMany(s => s.TagTests)
                .HasForeignKey(sc => sc.TestId);

            modelBuilder.Entity<TagTest>()
                .HasOne(sc => sc.Test)
                .WithMany(c => c.TagTests)
                .HasForeignKey(sc => sc.TagId);

            /*   modelBuilder.Entity<QuestionOfTest>()
             .HasOne(s => s.ClosedQuestion)
             .WithOne(ad => ad.QuestionOfTest)
             .HasForeignKey<ClosedQuestion>(ad => ad.QuestionOfTestId);

         modelBuilder.Entity<LevelOfTest>()
             .HasOne(p => p.Test)
             .WithMany(t => t.LevelOfTests)
             .HasForeignKey(s => s.TestId)
             .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<QuestionOfTest>()
               .HasOne(p => p.LevelOfQuestion)
               .WithMany(t => t.QuestionOfTests)
               .HasForeignKey(s => s.LevelOfQuestionId)
               .OnDelete(DeleteBehavior.Cascade);*/


            base.OnModelCreating(modelBuilder);
        }
    }



    public class User : IdentityUser
    {
        public int Year { get; set; }
    }

   
}
