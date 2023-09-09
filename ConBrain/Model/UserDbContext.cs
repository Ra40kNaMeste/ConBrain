using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace ConBrain.Model
{
    /// <summary>
    /// База данных с параметрами пользователей
    /// </summary>
    public class UserDbContext:DbContext
    {
        public UserDbContext(string connect):base()
        {
            _connect = connect;
            Database.EnsureCreated();
        }

        public DbSet<Person> People { get; set; }
        private readonly string _connect;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connect);
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Person>().Property(i => i.Nick).IsRequired().IsUnicode();
            builder.Entity<Person>().Property(i=>i.Phone).IsRequired().IsUnicode();
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Nick { get; set; } = "";
        public string Name { get; set; } = "";
        public string Family { get; set; } = "";
        public string? LastName { get; set; }
        public string Phone { get; set; } = "";
        public IEnumerable<Person>? Friends { get; set; }
        public IEnumerable<Message>? Messages { get; set; }
    }

    public class Message
    {
        public Person Target { get; set; }
        public Person Sender { get; set; }
        public string Content { get; set; }
    }

}
