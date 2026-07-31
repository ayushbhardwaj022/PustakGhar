using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_COM_Models
{
    public class OrderDetail
    {
        public int id { get; set; }
        public int OrderHeaderid { get; set; }
        [ForeignKey("OrderHeaderid")]
        public OrderHeader OrderHeader { get; set; }
        public int Productid { get; set; }
        [ForeignKey("Productid")]
        public Product Product { get; set; }
        public int count { get; set; }
        public double price { get; set; }
    }
}
