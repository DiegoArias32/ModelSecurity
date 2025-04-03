namespace Entity.DTOs
{
    public class RolUserDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RolId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime DeleteAt { get; set; }
    }
}
