using System;

namespace Entity.DTOs
{
    public class PqrDto
    {
        public int PqrId { get; set; }
        public required string PqrType { get; set; }
        public required string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public required string PqrStatus { get; set; }
        public DateTime ResolutionDate { get; set; }
        public int WorkerId { get; set; }
        public int ClientId { get; set; }
    }

}
