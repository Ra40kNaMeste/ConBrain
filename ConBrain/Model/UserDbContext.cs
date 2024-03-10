using ConBrain.Extensions;
using ConBrain.TagHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
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
        public DbSet<Dialog> Dialogs { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;

        public DbSet<PersonData> PersonData { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FriendPerson>()
                .HasOne(p => p.Target)
                .WithMany(p => p.Friends)
                .HasForeignKey(p => p.TargetId);

            builder.Entity<FriendPerson>()
                .HasOne(p => p.Friend)
                .WithMany(p => p.Subscribers)
                .HasForeignKey(p => p.FriendId);
            builder.Entity<FriendPerson>().HasKey(p => new{p.TargetId, p.FriendId});

            builder.Entity<PersonData>().Property(i => i.AvatarPath).HasDefaultValue("default.jpg");

            builder.Entity<Dialog>().Property(i=>i.Name).IsUnicode(true);
        }
    }

    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(5)]
        [MaxLength(50)]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; } = "";
        public List<FriendPerson> Friends { get; set; } = new();
        public List<FriendPerson> Subscribers { get; set; } = new();
        public List<Dialog> Dialogs { get; set; }
        public PersonData Data { get; set; }

    }

    public class FriendPerson
    {
        public Person Friend { get; set; }
        public int FriendId { get; set; }
        public Person Target { get; set; }
        public int TargetId { get; set; }

    }

    public class Dialog
    {
        public int Id { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        [StringLength(50, MinimumLength = 5)]
        public string Name { get; set; }
        public List<Person> Members { get; set; } = new();
        public List<Message> Messages { get; set; } = new();
    }

    public class Message
    {
        public int Id { get; set; }
        
        public DateTime DateTime { get; set; }

        public string Body { get; set; }
        public Person? Sender { get; set; }

        public int DialogId { get; set; }
        public Dialog Dialog { get; set; }
    }


    public class PersonData
    {
        public PersonData() { }

        [Key]
        [NonCopy]
        public int Id { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        [StringLength(50, MinimumLength = 5)]
        [TagHelpers.Display("Nick", Type = "text", Classes = new string[] { "sendInput" })]
        public string Nick { get; set; } = "";

        [NonCopy]
        public string? AvatarPath { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        [StringLength(50, MinimumLength = 1)]
        [TagHelpers.Display("Name", Type = "text", Classes = new string[] { "sendInput" })]
        public string Name { get; set; } = "";

        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        [StringLength(50, MinimumLength = 1)]
        [TagHelpers.Display("Family", Type = "text", Classes = new string[] { "sendInput" })]
        public string Family { get; set; } = "";

        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        [StringLength(50, MinimumLength = 1)]
        [TagHelpers.Display("Second name", Type = "text", Classes = new string[] { "sendInput" })]
        public string? SecondName { get; set; }

        [Required]
        [MinLength(7)]
        [MaxLength(50)]
        [StringLength(50, MinimumLength = 7)]
        [RegularExpression(@"^\+[0-9]+\([0-9]{3}\)[0-9]{3}(-[0-9]{2}){2}$", ErrorMessage = "The Phone is by format +xx(xxx)xxx-xx-xx")]
        [TagHelpers.Display("Phone", Type = "tel", Classes =new string[] {"phone", "sendInput" })]
        public string? Phone { get; set; }

        [NonCopy]
        public int PersonId { get; set; }
    }

    public class MessageSavedMementor
    {
        public MessageSavedMementor(int id, DateTime dateTime, string body, string? sender = null) 
        {
            Id = id;
            DateTime = dateTime;
            Body = body;
            Sender = sender;
        }
        public MessageSavedMementor(Message message)
        {
            Id = message.Id;
            DateTime = message.DateTime;
            Body = message.Body;
            Sender = message.Sender?.Data.Nick;
        }
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Body { get; set; }
        public string? Sender { get; set; }
    }
}
