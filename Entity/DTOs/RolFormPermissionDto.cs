using System;

namespace Entity.DTOs
{
    public class RolFormPermissionDto
    {
        public int Id { get; set; }
        public int RolId { get; set; }
        public int FormId { get; set; }
        public int PermissionId { get; set; }
    }
}
