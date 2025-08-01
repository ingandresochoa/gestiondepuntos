using System;

namespace GestionPuntosAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public decimal Points { get; set; } = 0;
        public bool IsAdmin { get; set; } = false;
        public DateTime CreatedAt { get; set; }
    }
}