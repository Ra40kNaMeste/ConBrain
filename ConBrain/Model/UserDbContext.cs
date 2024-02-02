using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
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
        public DbSet<FriendPerson> FreindsList { get; set; } = null!;

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

            builder.Entity<FriendPerson>()
                .HasOne(p => p.Target)
                .WithMany(p => p.Friends)
                .HasForeignKey(p => p.TargetId);

            builder.Entity<FriendPerson>()
                .HasOne(p => p.Friend)
                .WithMany(p => p.Subscribers)
                .HasForeignKey(p => p.FriendId);
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Password { get; set; } = "";
        public List<FriendPerson> Friends { get; set; } = new();
        public List<FriendPerson> Subscribers { get; set; } = new();
        public string Nick { get; set; } = "";
        public string? AvatarPath { get; set; }
        public string Name { get; set; } = "";
        public string Family { get; set; } = "";
        public string? LastName { get; set; }
        public string Phone { get; set; } = "";
    }

    public class FriendPerson
    {
        public int Id { get; set; }
        public Person Friend { get; set; }
        public int FriendId { get; set; }
        public Person Target { get; set; }
        public int TargetId { get; set; }

    }


    public class PersonSavedMementor
    {
        public PersonSavedMementor() { }
        public PersonSavedMementor(string nick, string? avatarPath, string name, string family, string? lastName, string phone)
        {
            Nick = nick;
            AvatarPath = avatarPath;
            Name = name;
            Family = family;
            LastName = lastName;
            Phone = phone;
        }
        public PersonSavedMementor(Person person)
        {
            Nick = person.Nick;
            AvatarPath = person.AvatarPath;
            Name = person.Name;
            Family = person.Family;
            LastName = person.LastName;
        }

        public string Nick { get; set; } = "";
        public string? AvatarPath { get; set; }
        public string Name { get; set; } = "";
        public string Family { get; set; } = "";
        public string? LastName { get; set; }
        public string Phone { get; set; } = "";
    }
}
