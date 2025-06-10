using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectInteractionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectInteractionsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim != null ? int.Parse(userIdClaim) : 0;
        }

        

        // ✅ Add a Comment
        [HttpPost("addComment/{projectId}")]
        [Authorize]
        public async Task<IActionResult> AddComment(int projectId, [FromBody] CommentDto dto)
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized("User not authenticated.");

            if (string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest("Comment content cannot be empty.");

            // تأكد إن المشروع موجود
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
                return NotFound("Project not found.");

            var comment = new Comment
            {
                content = dto.Content,
                commentOwnerId = userId,
                ProjectId = projectId, // ✅ الربط هنا مهم
                date = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Comment added successfully", Content = dto.Content });
        }

        // ✅ Update My Comment
        [HttpPut("{projectId}/comments/{commentId}")]
        [Authorize]
        public async Task<IActionResult> UpdateMyComment(int projectId, int commentId, [FromBody] UpdateCommentDto dto)
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized("User not authenticated.");

            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null || comment.commentOwnerId != userId)
                return NotFound("Comment not found or unauthorized.");

            comment.content = dto.NewContent;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Comment updated successfully", NewContent = dto.NewContent });
        }

        // ✅ Delete My Comment
        [HttpDelete("projects/{projectId}/MyComments/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteMyComment(int projectId, int commentId)
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized("User not authenticated.");

            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null || comment.commentOwnerId != userId)
                return NotFound("Comment not found or unauthorized.");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok("Comment deleted successfully.");
        }

        // ✅ Delete a Comment on My Project
        [HttpDelete("{projectId}/Comments/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteCommentOnMyProject(int projectId, int commentId)
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized("User not authenticated.");

            var project = await _context.Projects.FindAsync(projectId);
            if (project == null || project.ownerId != userId)
                return NotFound("Project not found or unauthorized.");

            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null) return NotFound("Comment not found.");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok("Comment deleted successfully.");
        }

        // ✅ Admin Delete Comment
        [HttpDelete("api/projects/{projectId}/MyComments/{commentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOneComment(int projectId, int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null) return NotFound("Comment not found.");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok("Admin deleted the comment.");
        }

        [HttpGet("{projectId}/comments")]
        public async Task<IActionResult> GetAllComments(int projectId)
        {
            var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists) return NotFound("Project not found.");

            var comments = await _context.Comments
         .Where(c => _context.Projects.Any(p => p.ownerId == c.commentOwnerId && p.Id == projectId))
         .ToListAsync();


            if (!comments.Any()) return NotFound("No comments found for this project.");

            return Ok(comments);
        }






        [HttpPost("{projectId}/addlike")]
        [Authorize]
        public async Task<IActionResult> AddLike(int projectId)
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized("User not authenticated.");

            // Check if the user has already liked the project
            var existingLike = await _context.Likes.FirstOrDefaultAsync(l => l.projectId == projectId && l.likeOwnerId == userId);
            if (existingLike != null)
            {
                return Conflict("User has already liked this project.");
            }

            var like = new Like { likeOwnerId = userId, projectId = projectId };
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Like added successfully", LikeId = like.Id });
        }

        [HttpDelete("deleteMyLike/{project_id}/{like_id}")]
        [Authorize]
        public async Task<IActionResult> DeleteMyLike(int projectId, int likeId)
        {
            int userId = GetUserId();
            if (userId == 0)
                return Unauthorized("User not authenticated.");

            // Find the user's like on the project
            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.projectId == projectId && l.Id == likeId && l.likeOwnerId == userId);

            if (like == null)
                return NotFound("Like not found or unauthorized.");

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return Ok("Like removed successfully.");
        }


        [Authorize]
        [HttpGet("getAllLikes/{projectId}")]
        public async Task<IActionResult> GetAllLikes(int projectId)
        {
            var likes = await _context.Likes
                .Where(l => l.projectId == projectId) // Ensure likes belong to the project
                .Select(l => new { l.Id, l.likeOwnerId }) // Return more useful data
                .ToListAsync();

            if (!likes.Any())
                return NotFound("No likes found for this project.");

            return Ok(new { LikesCount = likes.Count, Likes = likes });
        }

        [HttpPost("{projectId}/comments/{commentId}/likes")]
        [Authorize]
        public async Task<IActionResult> AddLikeToComment(int projectId, int commentId)
        {
            int userId = GetUserId();
            if (userId == 0) return Unauthorized("User not authenticated.");

            // Check if the user has already liked the comment
            var existingLike = await _context.Likes.FirstOrDefaultAsync(l => l.commentId == commentId && l.likeOwnerId == userId);
            if (existingLike != null)
            {
                return Conflict("User has already liked this comment.");
            }

            // Ensure the comment exists
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                return NotFound("Comment not found.");

            var like = new Like { likeOwnerId = userId, commentId = commentId };

            _context.Likes.Add(like);
            comment.numberOfCommentLikes++; // Increment comment likes count
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Like added to comment successfully", LikeId = like.Id });
        }




        [HttpGet("{projectId}/ShowAllLikesOnComment/{commentId}")]
        public async Task<IActionResult> ShowAllLikesOnComment(int projectId, int commentId)
        {
            var likes = await _context.Likes
                .Where(l => l.commentId == commentId) // Ensures only likes on this comment are counted
                .ToListAsync();

            return Ok(new { LikesCount = likes.Count, Likes = likes });
        }


        [HttpDelete("{projectId}/{commentId}/like/{likeId}/DeleteMyLikeOnComment")]
        [Authorize]
        public async Task<IActionResult> DeleteMyLikeOnComment(int projectId, int commentId, int likeId)
        {
            int userId = GetUserId();
            if (userId == 0)
                return Unauthorized("User not authenticated.");

            var comment = await _context.Comments
                .Include(c => c.likesOfComment)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
                return NotFound("Comment not found.");

            var like = comment.likesOfComment.FirstOrDefault(l => l.Id == likeId);
            if (like == null)
                return NotFound("Like not found.");

            if (like.likeOwnerId != userId)
                return Forbid("You are not authorized to delete this like.");

            comment.likesOfComment.Remove(like);
            comment.numberOfCommentLikes = comment.likesOfComment.Count;

            await _context.SaveChangesAsync();

            return Accepted(new
            {
                message = "Like removed from comment.",
                deletedLike = new { like.Id, like.likeOwnerId },
                remainingLikes = comment.likesOfComment.Select(l => new { l.Id, l.likeOwnerId })
            });
        }





        // DTOs
        public class CommentDto
        {
            public string Content { get; set; }
        }

        public class UpdateCommentDto
        {
            public string NewContent { get; set; }
        }
    }
}
