using Final.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SysAction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey("user")]
    public int userId { get; set; }
    public User user { get; set; }

    public string action { get; set; }

    public DateTime date { get; set; } = DateTime.UtcNow;

    public DateTime creationTime { get; set; } = DateTime.UtcNow;
    public DateTime lastModified { get; set; } = DateTime.UtcNow;
}
