using System;

namespace GestionPuntosAPI.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // "Grant", "Redeem"
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
    }
}