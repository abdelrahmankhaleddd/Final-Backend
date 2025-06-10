using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Notification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]

    public string eventOwner { get; set; }
    public string projectOwner { get; set; }

    [ForeignKey("project")]
    public int projectId { get; set; }
    public Project project { get; set; }

    public string content { get; set; }

    public DateTime date { get; set; } = DateTime.UtcNow;

    public DateTime creationTime { get; set; } = DateTime.UtcNow;
    public DateTime lastModified { get; set; } = DateTime.UtcNow;
}
