namespace Entity.Model
{
    public class RolUser  
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int RolId { get; set; }
        public Rol Rol { get; set; }
        public DateTime? DeleteAt { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
