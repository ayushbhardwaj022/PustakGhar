using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_COM_Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> Listcart { get; set; }
        public OrderHeader OrderHeader { get; set; }

        public List<AddressVM> RecentAddresses { get; set; }
    }
        public class AddressVM
    {
        
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }
}
    


