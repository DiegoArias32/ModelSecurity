namespace Entity.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
    }
}
