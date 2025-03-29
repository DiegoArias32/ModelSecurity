using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Enum
{
    public enum Permission
    {
        [Display(Name = "Create")]
        Neighbourhoods = 1,

        [Display(Name = "Read")]
        CreateNeighbourhoods = 2,

        [Display(Name = "Update")]
        EditNeighbourhoods = 3,

        [Display(Name = "Delete")]
        DeleteNeighbourhoods = 4,
    }
}