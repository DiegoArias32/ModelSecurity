using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Worker
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
