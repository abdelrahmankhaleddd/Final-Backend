using Final.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public enum projectStatus
{
    pending,
    accepted,
    rejected
}

public class Project
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string projectName { get; set; } = "object";
    public string category { get; set; } = "General";
    public string faculty { get; set; } = "General";
    public DateTime date { get; set; } = DateTime.UtcNow;

    public projectStatus status { get; set; } = projectStatus.pending;

    public int numberOfComments { get; set; }
    public ICollection<Comment> comments { get; set; } = new List<Comment>();

    public ICollection<Like> likes { get; set; } = new List<Like>();
    public int numberOfLikes { get; set; }

    public string description { get; set; }

    [Required]
    public string pdf { get; set; }

    [ForeignKey("owner")]
    public int ownerId { get; set; }
    public User owner { get; set; }
    public ICollection<FavList> FavLists { get; set; } = new List<FavList>();

    public DateTime creationTime { get; set; } = DateTime.UtcNow;
    public DateTime lastModified { get; set; } = DateTime.UtcNow;
}
public class Comment
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("commentOwner")]
    public int commentOwnerId { get; set; }
    public User commentOwner { get; set; }

    public string content { get; set; }
    public DateTime date { get; set; } = DateTime.UtcNow;

    public ICollection<Like> likesOfComment { get; set; } = new List<Like>();
    public int numberOfCommentLikes { get; set; }

    // ✅ الربط بمشروع
    [ForeignKey("Project")]
    public int? ProjectId { get; set; }
    public Project Project { get; set; }

}


public class Like
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("likeOwner")]
    public int likeOwnerId { get; set; }
    public User likeOwner { get; set; }

    [ForeignKey("comment")]
    public int? commentId { get; set; }
    public Comment comment { get; set; }

    [ForeignKey("project")]
    public int? projectId { get; set; }
    public Project project { get; set; }
}

