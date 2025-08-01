namespace GestionPuntosAPI.DTOs{
    public class UserListDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public decimal Points { get; set; }
        public bool IsAdmin { get; set; }
    }
}