using System;
using System.Collections.Generic;

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

        // Relación uno a uno con User
        public User User { get; set; }

        // Relación uno a muchos con WorkerLogin
        public ICollection<WorkerLogin> WorkerLogins { get; set; }
    }
}
