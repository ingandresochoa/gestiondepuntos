namespace GestionPuntosAPI.DTOs
{
    public class TransactionResponseDTO
    {
        public decimal Amount { get; set; }
        public string Type { get; set; } // "Grant", "Redeem"
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}