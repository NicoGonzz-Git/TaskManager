using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager_API.Data;
using TaskManager_API.Models.Domain;
using TaskManager_API.DTOs;

namespace TaskManager_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]

        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();

            var usersDTO = users.Select(user => new UsersDto
            {
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role
            }).ToList();

            return Ok(new
            {
                Sucess = true,
                Message = "Users retrieved sucessfully",
                Data = usersDTO
            });
        }

        [HttpDelete("{id:guid}")]

        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = $"User with ID {id} not found."
                });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = $"User with ID {id} deleted successfully."
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUser request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Invalid user data.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                LastName = request.LastName,
                Email = request.Email,
                Role = request.Role,
                CreatedDate = DateTime.UtcNow,
                UserImageURL = request.UserImageURL
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Success = true,
                Message = "User created successfully.",
                Data = user
            });
        }

    }  

}