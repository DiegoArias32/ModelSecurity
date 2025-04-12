using System;

namespace Entity.Model
{
    public class Permission
    {
        public int Id { get; set; }

        public bool Can_Read { get; set; }
        public bool Can_Create { get; set; }
        public bool Can_Update { get; set; }
        public bool Can_Delete { get; set; }

        public DateTime CreateAt { get; set; }
        public DateTime? DeleteAt { get; set;}
    }
}
