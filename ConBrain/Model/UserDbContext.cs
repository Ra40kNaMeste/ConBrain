using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace ConBrain.Model
{
    /// <summary>
    /// База данных с параметрами пользователей
    /// </summary>
    public class UserDbContext:DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options):base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Person> People { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Person>().Property(i => i.Nick).IsRequired().HasMaxLength(100);
            builder.Entity<Person>().HasIndex(i => i.Nick).IsUnique();

            builder.Entity<Person>().Property(i => i.Name).HasMaxLength(100);
            builder.Entity<Person>().Property(i => i.Family).HasMaxLength(100);
            builder.Entity<Person>().Property(i => i.LastName).HasMaxLength(100);

            builder.Entity<Person>().Property(i=>i.Phone).IsRequired();
            builder.Entity<Person>().HasIndex(i => i.Phone).IsUnique();


            builder.Entity<Message>().HasKey(i => i.Id);
            builder.Entity<Message>().HasOne(i => i.Sender).WithMany(i => i.SendedMessages);
            builder.Entity<Message>().HasOne(i => i.Target).WithMany(i => i.Messages);
        }
    }

    public class Person
    {
        public Person() 
        {
            Friends = new();
            SendedMessages = new();
            Messages = new();
        }
        public int Id { get; set; }
        public string Nick { get; set; } = "";
        public string? AvatarPath { get; set; }
        public string Name { get; set; } = "";
        public string Family { get; set; } = "";
        public string? LastName { get; set; }
        public string Phone { get; set; } = "";
        public string Password { get; set; } = "";
        public List<Person> Friends { get; set; } = null!;
        public List<Message> SendedMessages { get; set; } = null!;
        public List<Message> Messages { get; set; } = null!;
    }

    public class Message
    {
        public Message() { }
        public int Id { get; set; }
        public Person? Target { get; set; }
        public Person? Sender { get; set; }
        public string Content { get; set; } = "";
    }

}
