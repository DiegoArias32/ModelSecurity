using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class WorkerLogin
    {
        public int LoginId { get; set; }
        public int WorkerId { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public DateTime CreationDate { get; set; }
        public required string Status { get; set; }

        public required Worker Worker { get; set; }
    }

}
