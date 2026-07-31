using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_COM_Models
{
    public class ShoppingCart

    {
        public ShoppingCart() { count = 1; }
                                                                                            
        public int id { get; set; }
        public string ApplicationUserid { get; set; }
        [ForeignKey("ApplicationUserid")]
        public ApplicationUser ApplicationUser { get; set; }
        public int Productid { get; set; }
        [ForeignKey("Productid")]
        public Product Product{ get; set; }
        public int count { get; set; }
        [NotMapped]
        public double price { get; set; }



    }
    
}
