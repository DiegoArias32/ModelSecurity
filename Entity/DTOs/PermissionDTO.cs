namespace Entity.DTOs
{
    public class PermissionDTO
    {
        public int Id { get; set; }
        public bool CanRead { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }
}
