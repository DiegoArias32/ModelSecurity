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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdentityDocument { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public int Phone { get; set; }
        public DateTime? HireDate { get; set; }

        // Relación inversa con User
        public User User { get; set; }
    }

}
