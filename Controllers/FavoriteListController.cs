
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Final.Models;
using Final.Repositories;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Final.Controllers
{
    [Route("api/favorites")]
    [ApiController]

    public class FavoriteListController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ISystemActionRepository _sysActionRepository;

        public FavoriteListController(AppDbContext context, ISystemActionRepository sysActionRepository)
        {
            _context = context;
            _sysActionRepository = sysActionRepository;
        }
        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddProjectToFavoriteList([FromBody] AddFavoriteDto addFavoriteDto)
        {
            try
            {
                var loggedUserId = int.Parse(User.GetUserId());
                var loggedUser = await _context.Users.FindAsync(loggedUserId);

                if (loggedUser == null)
                    return Unauthorized(new { message = "Logged-in user not found." });

                var intendedProject = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == addFavoriteDto.ProjectID);

                if (intendedProject == null)
                    return NotFound(new { message = "Project not found or has been deleted." });

                if (intendedProject.status == projectStatus.pending)
                    return BadRequest(new { message = "You can't add a pending project to your favorite list." });

                // Check if already added
                bool alreadyAdded = await _context.FavLists.AnyAsync(f =>
                    f.ProjectId == addFavoriteDto.ProjectID && f.UserId == loggedUserId);

                if (alreadyAdded)
                    return Conflict(new { message = "This project is already in your favorite list." });

                // Add new entry to FavList
                var newFav = new FavList
                {
                    ProjectId = addFavoriteDto.ProjectID,
                    UserId = loggedUserId,
                    AddedDate = DateTime.UtcNow
                };

                _context.FavLists.Add(newFav);



                await _context.SaveChangesAsync();

                await _sysActionRepository.LogActionAsync($"{loggedUser.email} added project {intendedProject.projectName} to their favorite list", loggedUserId);

                return Ok(new { message = "Project added to favorite list successfully." });
            }
            catch (Exception error)
            {
                return StatusCode(500, new { message = error.Message });
            }
        }


        [Authorize]
        [HttpDelete("remove/{projectId}")]
        public async Task<IActionResult> RemoveProjectFromFavoriteList(int projectId)
        {
            try
            {
                var loggedUserId = int.Parse(User.GetUserId());
                var loggedUser = await _context.Users.FindAsync(loggedUserId);

                if (loggedUser == null)
                    return Unauthorized(new { message = "Logged-in user not found." });

                var intendedProject = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == projectId);

                if (intendedProject == null)
                    return NotFound(new { message = "Project not found or has been deleted." });

                var favEntry = await _context.FavLists.FirstOrDefaultAsync(f =>
                    f.ProjectId == projectId && f.UserId == loggedUserId);

                if (favEntry == null)
                    return Conflict(new { message = "This project is not in your favorite list." });

                _context.FavLists.Remove(favEntry);
                await _context.SaveChangesAsync();

                await _sysActionRepository.LogActionAsync($"{loggedUser.email} removed project {intendedProject.projectName} from their favorite list", loggedUserId);

                return Ok(new { message = "Project removed from favorite list successfully." });
            }
            catch (Exception error)
            {
                return StatusCode(500, new { message = error.Message });
            }
        }


        [HttpGet("my")]
        public async Task<IActionResult> ShowMyFavoriteList()
        {
            try
            {
                var loggedUserId = int.Parse(User.GetUserId());

                var myFavoriteProjects = await _context.FavLists
                    .Where(f => f.UserId == loggedUserId)
                    .Include(f => f.Project)
                        .ThenInclude(p => p.owner)
                    .Select(f => f.Project)
                    .ToListAsync();

                await _sysActionRepository.LogActionAsync(
                    $"{User.FindFirst(ClaimTypes.Email)?.Value} viewed their favorite list",
                    loggedUserId
                );

                return Ok(new
                {
                    projects_number = myFavoriteProjects.Count,
                    myFavoriteList = myFavoriteProjects
                });
            }
            catch (Exception error)
            {
                return StatusCode(500, new { message = error.Message });
            }
        }




        [HttpGet("{project_ID}/users")]
        public async Task<IActionResult> GetAllUsersAddedThisProjToFavList(int project_ID)
        {
            try
            {
                var intendedProject = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == project_ID);

                if (intendedProject == null)
                    return NotFound(new { message = "Project not found or has been deleted." });

                // جيب كل اليوزرز اللي ضايفين المشروع ده من جدول FavList
                var usersInFavorites = await _context.FavLists
                    .Where(f => f.ProjectId == project_ID)
                    .Include(f => f.User)
                    .Select(f => new
                    {
                        f.User.Id,
                        f.User.userName,
                        f.User.email,
                        f.User.role,
                        f.User.image
                    })
                    .ToListAsync();

                var userId = int.Parse(User.GetUserId());
                await _sysActionRepository.LogActionAsync(
                    $"{User.FindFirst(ClaimTypes.Email)?.Value} viewed all users who added this project to their favorite list",
                    userId);

                return Ok(new
                {
                    number_Of_users_added_this_project_to_favorite_list = usersInFavorites.Count,
                    users_added_this_project_to_favorite_list = usersInFavorites
                });
            }
            catch (Exception error)
            {
                return StatusCode(500, new { message = error.Message });
            }
        }
    }



    // DTOs
    public class AddFavoriteDto
    {
        public int ProjectID { get; set; }
    }

    public class RemoveFavoriteDto
    {
        public int ProjectID { get; set; }

    }

}
