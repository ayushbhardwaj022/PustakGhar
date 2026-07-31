using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_COM_Models
{
    public class CoverType
    {
        public int id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
