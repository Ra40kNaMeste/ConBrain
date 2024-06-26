﻿using ConBrain.Extensions;
using ConBrain.TagHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text.Json.Serialization;

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
        public DbSet<Image> Images { get; set; } = null!;

        public DbSet<PersonData> PersonData { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PersonData>().HasIndex(i => i.Nick).IsUnique();
            builder.Entity<PersonData>().HasIndex(i => i.Phone).IsUnique();

            builder.Entity<FriendPerson>()
                .HasOne(p => p.Target)
                .WithMany(p => p.Friends)
                .HasForeignKey(p => p.TargetId);

            builder.Entity<FriendPerson>()
                .HasOne(p => p.Friend)
                .WithMany(p => p.Subscribers)
                .HasForeignKey(p => p.FriendId);
            builder.Entity<FriendPerson>().HasKey(p => new{p.TargetId, p.FriendId});

            builder.Entity<Dialog>().Property(i=>i.Name).IsUnicode(true);
        }
    }

    public class Person
    {
        [Key]
        public int Id { get; set; }

        [JsonIgnore]
        [Required]
        [DataType(DataType.Password)]
        [MinLength(5)]
        [MaxLength(50)]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; } = "";
        [JsonIgnore]
        public List<FriendPerson> Friends { get; set; } = new();
        [JsonIgnore]
        public List<FriendPerson> Subscribers { get; set; } = new();
        [JsonIgnore]
        public List<Dialog> Dialogs { get; set; } = new();
        [JsonIgnore]
        public List<Image> Images { get; set; } = new();
        public PersonData Data { get; set; }

    }

    public class FriendPerson
    {
        public Person Friend { get; set; }
        public int FriendId { get; set; }
        [JsonIgnore]
        public Person Target { get; set; }
        [JsonIgnore]
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
        [JsonIgnore]
        public List<Message> Messages { get; set; } = new();
    }

    public class Message
    {
        public int Id { get; set; }
        
        public DateTime DateTime { get; set; }

        public string Body { get; set; }

        public int? SenderId { get; set; }
        public Person? Sender { get; set; }

        [JsonIgnore]
        public int DialogId { get; set; }
        [JsonIgnore]
        public Dialog Dialog { get; set; }
    }

    public class Image: SecurityLevelObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public string FileExtension { get; set; }
        
        [JsonIgnore]
        public byte[] Data { get; set; }
        public decimal Size { get; set; }

        public SecurityLevel SecurityLevel { get; set; }

        [JsonIgnore]
        public int OwnerId { get; set; }
        [JsonIgnore]
        public Person Owner { get; set; }
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
        public string Nick { get; set; } = "";

        public int? AvatarId { get; set; }
        [NonCopy]
        public Image? Avatar { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; } = "";

        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        [StringLength(50, MinimumLength = 1)]
        public string Family { get; set; } = "";

        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        [StringLength(50, MinimumLength = 1)]
        public string? SecondName { get; set; }

        [Required]
        [MinLength(7)]
        [MaxLength(50)]
        [StringLength(50, MinimumLength = 7)]
        [RegularExpression(@"^\+[0-9]+\([0-9]{3}\)[0-9]{3}(-[0-9]{2}){2}$", ErrorMessage = "The Phone is by format +xx(xxx)xxx-xx-xx")]
        public string? Phone { get; set; }

        [NonCopy]
        public int PersonId { get; set; }
    }
}
