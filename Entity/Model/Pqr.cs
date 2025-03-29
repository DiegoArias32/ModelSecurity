using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Pqr
    {
        public int PqrId { get; set; }
        public required string PqrType { get; set; }
        public required string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public required string PqrStatus { get; set; }
        public DateTime ResolutionDate { get; set; }
        public int WorkerId { get; set; }
        public int ClientId { get; set; }

        public required Worker Worker { get; set; }
        public required Client Client { get; set; }
    }

}
