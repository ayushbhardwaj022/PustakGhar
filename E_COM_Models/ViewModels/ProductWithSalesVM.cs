using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_COM_Models.ViewModels
{
    public class ProductWithSalesVM
    {
        public Product Product { get; set; }
        public int SoldCount { get; set; }
        public int PurchaseDate { get; set; }
    }
}
