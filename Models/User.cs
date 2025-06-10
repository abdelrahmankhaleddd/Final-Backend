using System;
using Final.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Final.Models
{

    public enum UserRole
    {
        student,
        admin
    }

    public class User 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string userName { get; set; }

        [Required]
        [EmailAddress]
        [Column(TypeName = "varchar(255)")]
        public string email { get; set; }

        [Required]
        [MinLength(8)]
        public string password { get; set; }

        [Required]
        [EmailAddress]
        public string gmailAcc { get; set; }

        public string? passwordResetCode { get; set; }
        public DateTime? passwordResetExpires { get; set; }
        public bool? passwordResetVerified { get; set; }

        public bool active { get; set; } = true;

        [Required]
        public UserRole role { get; set; } = UserRole.student;

        public string? phone { get; set; }
        public string? bio { get; set; }

        public string image { get; set; } = "/app/user/defualt.jpg";
        public string? transcript { get; set; }

        public int age { get; set; } = 18;

        public addresses addresses { get; set; }

        [JsonIgnore]
        public bool isBlocked { get; set; } = false;

        public DateTime date { get; set; } = DateTime.UtcNow;
        public ICollection<FavList> FavLists { get; set; }

        public ICollection<Token> tokens { get; set; } = new List<Token>();

        public DateTime creationTime { get; set; } = DateTime.UtcNow;
        public DateTime lastModified { get; set; } = DateTime.UtcNow;

        public void HashPassword()
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                password = Convert.ToBase64String(sha256.ComputeHash(bytes));
            }
        }
    }

    [Owned] // ✅ This tells EF Core that Address is an owned entity
    public class addresses
    {
        public string? Country { get; set; }
        public string? cityOrTown { get; set; }
        public string? details { get; set; }
    }

    public class Token
    {
        [Key]
        public int Id { get; set; }

        public string value { get; set; }

        public DateTime expires { get; set; } = DateTime.UtcNow.AddHours(7);
    }
}
