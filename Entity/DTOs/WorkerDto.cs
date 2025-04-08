using System;

namespace Entity.DTOs
{
    public class WorkerDto
    {
        public int WorkerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdentityDocument { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public int Phone { get; set; }
        public DateTime? HireDate { get; set; }
        
    }

}
