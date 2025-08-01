namespace GestionPuntosAPI.DTOs {
    public class GrantDTO
    {
        public int UserId { get; set; }
        public int Points { get; set; }
        public string? Reason { get; set; }
    }
}