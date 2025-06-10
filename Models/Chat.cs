using Final.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Chat
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int chatNums { get; set; }

    public string chatName { get; set; }

    [ForeignKey("sender")]
    public int senderId { get; set; }
    public User sender { get; set; }

    [ForeignKey("receiver")]
    public int receiverId { get; set; }
    public User receiver { get; set; }

    public ICollection<Message> AllMessages { get; set; } = new List<Message>();

    public DateTime creationTime { get; set; } = DateTime.UtcNow;
    public DateTime lastModified { get; set; } = DateTime.UtcNow;
}

public class Message
{
    [Key]
    public int Id { get; set; }

    public string theMessage { get; set; }

    [ForeignKey("messageOwner")]
    public int messageOwnerId { get; set; }
    public User messageOwner { get; set; }

    public DateTime date { get; set; } = DateTime.UtcNow;
}
