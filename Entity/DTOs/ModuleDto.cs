    using System;

    namespace Entity.DTOs
    {
        public class ModuleDto
        {
            public int Id { get; set; }
            public required string Code { get; set; }
            public bool Active { get; set; }
        }
    }
