using System;

namespace Entity.DTOs
{
    public class WorkerDto
    {
        public int WorkerId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string IdentityDocument { get; set; }
        public required string JobTitle { get; set; }
        public required string Email { get; set; }
        public int Phone { get; set; }
        public DateTime? HireDate { get; set; }
    }

}
