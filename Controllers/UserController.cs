using Microsoft.AspNetCore.Mvc;
using GestionPuntosAPI.Data;
using GestionPuntosAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using GestionPuntosAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GestionPuntosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/User/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest("El correo ya está registrado.");
            }

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                FirstName = dto.FirstName,
                Points = 0,
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario registrado exitosamente." });
        }

        // POST: api/User/login
        [Authorize]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || user.PasswordHash != dto.Password)
            {
                return Unauthorized("Credenciales incorrectas.");
            }

            return Ok(new { message = "Inicio de sesión exitoso.", user.Id, user.Email, user.Points });
        }

        // GET: api/User/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return Ok(user);
        }

        // GET: api/user/profile
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Token inválido");

            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new 
                {
                    u.Id,
                    u.Email,
                    u.FirstName,
                    Puntos = u.Points
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("Usuario no encontrado");

            return Ok(user);
        }

        // GET: api/user/redeem
        [Authorize]
        [HttpPost("redeem")]
        public async Task<IActionResult> RedeemPoints([FromBody] RedeemDTO request)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Token inválido");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Usuario no encontrado");

            if (request.Points <= 0)
                return BadRequest("La cantidad debe ser mayor a cero.");

            if (user.Points < request.Points)
                return BadRequest("No tienes suficientes puntos.");

            // Actualizar puntos del usuario
            user.Points -= request.Points;

            // Registrar transacción
            var transaction = new Transaction
            {
                UserId = user.Id,
                Amount = request.Points,
                Type = "Redeem",
                Description = request.Description ?? "Redención de puntos",
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Puntos redimidos correctamente",
                PuntosActuales = user.Points
            });
        }

        // GET: api/user/history
        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> GetTransactionHistory()
        {
            // Obtener ID del usuario autenticado
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized("Token inválido.");

            // Buscar usuario
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            // Obtener historial de transacciones ordenado (más recientes primero)
            var history = await _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TransactionResponseDTO
                {
                    Amount = t.Amount,
                    Type = t.Type,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return Ok(history);
        }

        [HttpGet("debug-claims")]
        public IActionResult DebugClaims()
        {
            return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
        }

        // Métodos auxiliares para hash
        private string CreateHash(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string input, string storedHash)
        {
            var inputHash = CreateHash(input);
            return inputHash == storedHash;
        }
    }
}