namespace Entity.DTOs
{
    public class CreateUserDTO
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; } // ⚠️ Solo se usa en la creación
    }
}
