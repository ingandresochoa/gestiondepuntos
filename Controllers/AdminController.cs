using Microsoft.AspNetCore.Mvc;
using GestionPuntosAPI.Data;
using GestionPuntosAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using GestionPuntosAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GestionPuntosAPI.Controllers{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("grant")]
        public async Task<IActionResult> GrantPoints(GrantDTO dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                return NotFound("Usuario no encontrado");
            }

            user.Points += dto.Points;

            _context.Transactions.Add(new Transaction
            {
                UserId = user.Id,
                Amount = dto.Points,
                Type = "Grant",
                Description = dto.Reason ?? "Puntos otorgados por administrador",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Se otorgaron {dto.Points} puntos al usuario {user.FirstName}",
                totalPoints = user.Points
            });
        }

        // GET: api/admin/users
        [Authorize(Roles = "Admin")]
        [HttpGet("/api/admin/users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new UserListDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    Points = u.Points,
                    IsAdmin = u.IsAdmin
                })
                .ToListAsync();

            return Ok(users);
        }
    }

}