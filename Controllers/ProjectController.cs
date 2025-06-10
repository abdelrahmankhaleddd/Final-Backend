using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Final.Models;
using Final.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;
using System.Text.RegularExpressions;
using Final.DTOs;
using Final.Repositories;
using System.Security.Claims;

namespace Final.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // Requires authentication
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context; // Replace with your actual DbContext
        private readonly ILogger<ProjectController> _logger;
        private readonly IMailingService _emailService;
        private readonly ISystemActionRepository _sysActionRepository; // Assuming you have this
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProjectController(AppDbContext context, ILogger<ProjectController> logger, IMailingService emailService, ISystemActionRepository sysActionRepository, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _sysActionRepository = sysActionRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        [Authorize]
        [HttpPut("updateProjectData/{project_Id}")]
        public async Task<IActionResult> UpdateProjectData(string project_Id, [FromForm] UpdateProjectDto model)
        {
            try
            {
                var logedUserID = int.Parse(User.GetUserId());

                var existingProject = await _context.Projects.FindAsync(int.Parse(project_Id));
                if (existingProject == null)
                {
                    return NotFound(new { message = "Project not found." });
                }

                if (existingProject.ownerId != logedUserID)
                {
                    return StatusCode(403, new { message = "You are not authorized to update this project." });
                }

                // Update project properties
                existingProject.projectName = model.projectName ?? existingProject.projectName;
                existingProject.category = model.category ?? existingProject.category;
                existingProject.faculty = model.faculty ?? existingProject.faculty;
                existingProject.description = model.description ?? existingProject.description;

                // Update PDF file if provided
                //if (model.pdf != null && model.pdf.Length > 0)
                //{
                //    // Validate file extension
                //    string fileExtension = Path.GetExtension(model.pdf.FileName).ToLowerInvariant();
                //    if (fileExtension != ".pdf")
                //    {
                //        return BadRequest(new { message = "Invalid file extension. Only .pdf files are allowed." });
                //    }

                //    // Delete old PDF file
                //    var oldFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads-projects", Path.GetFileName(existingProject.pdf));
                //    if (System.IO.File.Exists(oldFilePath))
                //    {
                //        System.IO.File.Delete(oldFilePath);
                //    }

                //    // Save new PDF file
                //    string uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads-projects");
                //    if (!Directory.Exists(uploadsFolder))
                //    {
                //        Directory.CreateDirectory(uploadsFolder);
                //    }

                //    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.pdf.FileName;
                //    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                //    using (var fileStream = new FileStream(filePath, FileMode.Create))
                //    {
                //        await model.pdf.CopyToAsync(fileStream);
                //    }

                //    existingProject.pdf = $"{Request.Scheme}://{Request.Host}/Uploads-projects/{uniqueFileName}";
                //}
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "A concurrency error occurred while saving the project.");
                    return StatusCode(500, new { message = "Error updating project. Concurrency error occurred.  Please try again." });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "A database error occurred while saving the project.");
                    return StatusCode(500, new { message = "Error updating project. A database error occurred.  See the inner exception for details." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred while saving the project.");
                    return StatusCode(500, new { message = "Error updating project. An unexpected error occurred." });
                }
                await _sysActionRepository.LogActionAsync($"User {User.GetUserEmail()} updated project {existingProject.projectName}", logedUserID);

                return Ok(new { message = "Project updated successfully.", updatedProject = existingProject });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error updating project.");
                return StatusCode(500, new { message = error.Message });
            }
        }

        [HttpPut("updateProjectFile/{project_Id}")]
        public async Task<IActionResult> UpdateProjectFile(string project_Id, IFormFile sentFile)
        {
            try
            {
                var logedUserID = int.Parse(User.GetUserId());
                if (sentFile == null)
                {
                    return StatusCode(404, new { message = "no file found or check the extention of file (\".pdf\")" });
                }
                string fileExtension = Path.GetExtension(sentFile.FileName).ToLowerInvariant();
                if (fileExtension != ".pdf")
                {
                    return BadRequest(new { message = "Invalid file extension. Allowed extensions are  .pdf" });
                }
                string uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads-projects");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + sentFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await sentFile.CopyToAsync(fileStream);
                }
                var existingProject = await _context.Projects.FindAsync(int.Parse(project_Id));
                if (existingProject == null)
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return NotFound(new { message = "no project found with this ID" });
                }
                if (existingProject.ownerId != logedUserID)
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return StatusCode(403, new { message = "this project dosn't belong you to update" });
                }
                string oldFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads-projects", Path.GetFileName(existingProject.pdf));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                existingProject.pdf = $"{Request.Scheme}://{Request.Host}/Uploads-projects/{uniqueFileName}";
                existingProject.status = projectStatus.pending;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "A concurrency error occurred while saving the project.");
                    return StatusCode(500, new { message = "Error updating project. Concurrency error occurred.  Please try again." });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "A database error occurred while saving the project.");
                    return StatusCode(500, new { message = "Error updating project. A database error occurred.  See the inner exception for details." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred while saving the project.");
                    return StatusCode(500, new { message = "Error updating project. An unexpected error occurred." });
                }
                // Ensure User.GetUserEmail() is not null
                var userEmail = User.GetUserEmail();
                if (!string.IsNullOrEmpty(userEmail))
                {
                    await _context.Notifications.AddAsync(new Notification
                    {
                        eventOwner = userEmail,
                        projectOwner = userEmail,
                        projectId = existingProject.Id,
                        content = "this user updated the project file and in pending status",
                    });
                }
                else
                {
                    _logger.LogWarning("User email is null or empty. Notification not created.");
                }
                await _sysActionRepository.LogActionAsync($" {User.GetUserEmail()} updated his project file and in pending status", logedUserID);
                await _context.SaveChangesAsync();
                return StatusCode(201, new { message = "project file is updated and in pending status" });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error updating project.");
                return StatusCode(500, new { message = error.Message });
            }
        }
        [Authorize] // Requires authentication
        [HttpPost("createProject")]
        public async Task<IActionResult> CreateProject([FromForm] CreateProjectDto model)
        {
            try
            {
                var logedUserID = int.Parse(User.GetUserId());

                if (model.pdf == null || model.pdf.Length == 0)
                {
                    return StatusCode(400, new { message = "No file uploaded or file is empty." });
                }

                // Validate file extension
                string fileExtension = Path.GetExtension(model.pdf.FileName).ToLowerInvariant();
                if (fileExtension != ".pdf")
                {
                    return BadRequest(new { message = "Invalid file extension. Only .pdf files are allowed." });
                }

                // Save the file
                string uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads-projects");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.pdf.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.pdf.CopyToAsync(fileStream);
                }
                // Create new project
                var newProject = new Project
                {
                    projectName = model.projectName,
                    category = model.category,
                    faculty = model.faculty,
                    description = model.description,
                    ownerId = logedUserID,
                    pdf = $"{Request.Scheme}://{Request.Host}/Uploads-projects/{uniqueFileName}",
                };
                try
                {
                    _context.Projects.Add(newProject);
                    await _sysActionRepository.LogActionAsync($"User {User.GetUserEmail()} created a new project {newProject.projectName} and in pending status", logedUserID);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "A concurrency error occurred while saving the project.");
                    return StatusCode(500, new { message = "Error updating project. Concurrency error occurred.  Please try again." });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "A database error occurred while saving the project.");
                    return StatusCode(500, new { message = "Error updating project. A database error occurred.  See the inner exception for details." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred while saving the project.");
                    return StatusCode(500, new { message = "Error updating project. An unexpected error occurred." });
                }

                return StatusCode(201, new { message = "Project created and in pending status", newProject });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error creating project.");
                return StatusCode(500, new { message = error.Message });
            }
        }

        [HttpDelete("deleteMyProject/{project_Id}")]
        public async Task<IActionResult> DeleteMyProject(string project_Id)
        {
            try
            {
                var logedUserID = int.Parse(User.GetUserId());

                var existingProject = await _context.Projects.FindAsync(int.Parse(project_Id));
                if (existingProject == null)
                {
                    return NotFound(new { message = "no project found with this ID" });
                }
                if (existingProject.ownerId != logedUserID)
                {
                    return StatusCode(403, new { message = "this project dosn't belong you to delete" });
                }
                try
                {
                    _context.Projects.Remove(existingProject);
                    await _sysActionRepository.LogActionAsync($" {User.GetUserEmail()} deleted the project {existingProject.projectName}", logedUserID);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "A concurrency error occurred while saving the project.");
                    return StatusCode(500, new { message = "Error updating project. Concurrency error occurred.  Please try again." });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "A database error occurred while saving the project.");
                    return StatusCode(500, new { message = "Error updating project. A database error occurred.  See the inner exception for details." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred while saving the project.");
                    return StatusCode(500, new { message = "Error updating project. An unexpected error occurred." });
                }

                return StatusCode(202, new { message = "is deleted" });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error creating project.");
                return StatusCode(500, new { message = error.Message });
            }
        }
        [Authorize]
        [HttpGet("getSpacificProject/{project_Id}")]
        public async Task<IActionResult> GetSpacificProject(string project_Id)
        {
            try
            {
                var existingProject = await _context.Projects.Where(c => c.Id == int.Parse(project_Id))
                     .Include(c => c.comments)
                     .ThenInclude(c => c.commentOwner)
                    .Include(l => l.likes)
                     .ThenInclude(l => l.likeOwner)
                   .Include(o => o.owner)
                   .FirstOrDefaultAsync();
                if (existingProject == null)
                {
                    return StatusCode(404, new { message = "no project found or is deleted before " });
                }

                var ProjectOwner = await _context.Users.FindAsync(existingProject.ownerId);
                if (ProjectOwner == null)
                {
                    return StatusCode(404, new { message = "no ProjectOwner found or is deleted before " });
                }
                return StatusCode(202, new { data = existingProject, the_Project_owner = ProjectOwner });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error creating project.");
                return StatusCode(500, new { message = error.Message });
            }
        }
        [Authorize]
        [HttpGet("getAllProjects")] // بدل Post
        public async Task<IActionResult> GetAllProjects([FromQuery] string? category)
        {
            try
            {
                var allProjects = await _context.Projects
                   .Where(c => c.status == projectStatus.accepted)
                   .Include(c => c.comments)
                   .ThenInclude(c => c.commentOwner)
                   .Include(l => l.likes)
                   .ThenInclude(l => l.likeOwner)
                   .Include(o => o.owner)
                   .ToListAsync();

                if (allProjects == null || !allProjects.Any())
                {
                    return StatusCode(404, new { message = "no projects founded " });
                }

                var Projectscount = allProjects.Count();

                return StatusCode(200, new { message = $"number of projects: {Projectscount}", projects_data = allProjects });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error getting all projects.");
                return StatusCode(500, new { message = error.Message });
            }
        }
        [Authorize]
        [HttpGet("getAllmyProjects")]
        public async Task<IActionResult> GetAllmyProjects()
        {
            try
            {
                var logedUserID = int.Parse(User.GetUserId());

                var allMyProjects = await _context.Projects
                    .Where(c => c.ownerId == logedUserID)
                    .Include(c => c.comments)
                    .ThenInclude(c => c.commentOwner)
                    .Include(l => l.likes)
                    .ThenInclude(l => l.likeOwner)
                    .Include(o => o.owner)
                    .ToListAsync();

                if (allMyProjects == null || allMyProjects.Count == 0)
                {
                    return StatusCode(404, new { message = "No projects found." });
                }

                var MyProjectscount = await _context.Projects.Where(c => c.ownerId == logedUserID).CountAsync();

                // Logging action might be failing, consider removing or handling it
                try
                {
                    await _sysActionRepository.LogActionAsync($" {User.GetUserEmail()} show all his projects ", logedUserID);
                }
                catch (Exception logError)
                {
                    _logger.LogWarning(logError, "Logging action failed.");
                }

                return StatusCode(200, new { message = $"Number of My projects: {MyProjectscount}", data = allMyProjects });
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error fetching projects.");
                return StatusCode(500, new { message = error.ToString() });
            }
        }

        //[Authorize] // Requires authentication
        [HttpGet("getRandomProjects")]
        public async Task<IActionResult> GetRandomProjects()
        {
            try
            {
                // Get all accepted projects (compare against enum value)
                var acceptedProjects = await _context.Projects
                    .Where(p => p.status == projectStatus.accepted) // Use enum value
                    .ToListAsync();

                if (acceptedProjects.Count == 0)
                {
                    return StatusCode(404, new { message = "No accepted projects found." });
                }

                // Randomly pick 6 projects
                var randomProjects = acceptedProjects
                    .OrderBy(_ => Guid.NewGuid()) // Randomize the order
                    .Take(6) // Take only 6
                    .ToList();

                return StatusCode(202, randomProjects);
            }
            catch (Exception error)
            {
                // Log the error (consider adding logging)
                _logger.LogError(error, "Error fetching random projects.");
                return StatusCode(500, new { message = error.Message });
            }
        }

        [HttpPut("changeProjectStatus/{project_Id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ChangeProjectStatus(string project_Id, [FromBody] ChangeProjectStatusDto model)
        {
            if (!User.IsInRole("admin"))
            {
                return StatusCode(403, new { message = "You are not authorized to perform this action." });
            }

            if (!int.TryParse(project_Id, out int projectId))
            {
                return BadRequest(new { message = "Invalid project ID format." });
            }

            var existingProject = await _context.Projects.FindAsync(projectId);
            if (existingProject == null)
            {
                return NotFound(new { message = "No project found with this ID." });
            }

            var previousStatus = existingProject.status;
            existingProject.status = model.status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project status.");
                return StatusCode(500, new { message = "Error updating project status." });
            }

            var projectOwner = await _context.Users.FindAsync(existingProject.ownerId);
            if (projectOwner != null)
            {
                var userEmail = User.GetUserEmail();
                if (!string.IsNullOrEmpty(userEmail))
                {
                    string message = $"Hi {projectOwner.userName}, your project status changed from {previousStatus} to {model.status}.";

                    await _context.Notifications.AddAsync(new Notification
                    {
                        eventOwner = userEmail,
                        projectOwner = projectOwner.email,
                        projectId = existingProject.Id,
                        content = message
                    });

                    await _emailService.SendEmailAsync(projectOwner.gmailAcc, "Project Status Update", message);
                }
            }

            await _sysActionRepository.LogActionAsync($"{User.GetUserEmail()} changed project status: {existingProject.projectName}", int.Parse(User.GetUserId()));
            return Accepted(new { message = $"Status updated to {model.status}." });
        }

        [HttpDelete("deleteProject/{project_Id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteProject(string project_Id)
        {
            if (!User.IsInRole("admin"))
            {
                return Forbid();
            }

            if (!int.TryParse(project_Id, out int projectId))
            {
                return BadRequest(new { message = "Invalid project ID format." });
            }

            var existingProject = await _context.Projects.FindAsync(projectId);
            if (existingProject == null)
            {
                return NotFound(new { message = "No project found with this ID." });
            }

            _context.Projects.Remove(existingProject);
            await _context.SaveChangesAsync();

            return Accepted(new { message = "Project deleted successfully." });
        }

        [HttpGet("getAllPendingProjects")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllPendingProjects()
        {
            if (!User.IsInRole("admin"))
            {
                return Forbid();
            }

            var pendingProjects = await _context.Projects.Where(p => p.status == projectStatus.pending)
                .Include(p => p.owner)
                .ToListAsync();

            return Ok(new { count = pendingProjects.Count, data = pendingProjects });
        }

        [HttpPost("searchOnProject")]
        public async Task<IActionResult> SearchOnProject([FromBody] SearchOnProjectDTO model)
        {
            if (string.IsNullOrEmpty(model?.search))
            {
                return BadRequest(new { message = "Please enter a project name or description to search." });
            }

            var projects = await _context.Projects.Where(p => (p.projectName.Contains(model.search) || p.description.Contains(model.search)) && p.status == projectStatus.accepted)
                .ToListAsync();

            return projects.Any() ? Ok(projects) : NotFound(new { message = "No projects found matching the criteria." });
        }


        //DTOs
        public class UpdateProjectDto
        {
            public string projectName { get; set; }
            public string category { get; set; }
            public string faculty { get; set; }
            public string description { get; set; }
            // public IFormFile pdf { get; set; }
        }
        public class ChangeProjectStatusDto
        {
            public projectStatus status { get; set; }
        }
        public class GetAllProject
        {
            public string category { get; set; }
        }
        public class SearchOnProjectDTO
        {
            public string search { get; set; }
        }


        public class CreateProjectDto
        {
            public string projectName { get; set; }
            public string category { get; set; }
            public string faculty { get; set; }
            public string description { get; set; }
            public IFormFile pdf { get; set; }
        }
    }
}