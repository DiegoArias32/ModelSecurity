using System;

namespace Entity.DTOs
{
    public class ClientDto
    {
        public int ClientId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string IdentityDocument { get; set; }
        public required string ClientType { get; set; }
        public int Phone { get; set; }
        public required string Email { get; set; }
        public required string Address { get; set; }
        public int SocioeconomicStratification { get; set; }
        public DateTime RegistrationDate { get; set; }
    }

}
