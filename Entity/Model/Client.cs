using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class Client
    {
        public int ClientId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string IdentityDocument { get; set; }
        public required string ClientType { get; set; }
        public long Phone { get; set; }
        public required string Email { get; set; }
        public required string Address { get; set; }
        public int? SocioeconomicStratification { get; set; }
        public DateTime? RegistrationDate { get; set; } = DateTime.Now;
    }

}
