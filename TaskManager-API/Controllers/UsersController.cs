using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager_API.Data;
using TaskManager_API.Models.Domain;
using TaskManager_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Web.Resource;
using AutoMapper;

namespace TaskManager_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize()]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly ILogger<UsersController> _logger;
        private readonly IMapper _mapper;

        public UsersController(UserContext context, ILogger<UsersController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            var usersDTO = _mapper.Map<List<UsersDto>>(users);

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
            if (!ModelState.IsValid || request == null || string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Invalid user data.");

            var user = _mapper.Map<User>(request);
            user.Id = Guid.NewGuid();
            user.CreatedDate = DateTime.UtcNow;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UsersDto>(user);

            return Ok(new
            {
                Success = true,
                Message = "User created successfully.",
                Data = userDto
            });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUser request)
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

            _mapper.Map(request, user);
            await _context.SaveChangesAsync();

            var updatedUserDto = _mapper.Map<UsersDto>(user);

            return Ok(new
            {
                Success = true,
                Message = $"User updated successfully.",
                Data = updatedUserDto
            });
        }
    }
}