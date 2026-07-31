using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_COM_DataAccess.Repository.Irepository
{
    public interface IUnitofWork
    {
        IcategoryRepository category {  get; }//access to the Category repository
        ICoverTypeRepository coverType { get; }//access to the Covertype repository
        IProductRepository product { get; }//access to the product repository
        ICompanyRepository company { get; }
        IApplicationUserRepository ApplicationUser { get; }

        IShoppingCartRepository shoppingCart { get; }
        IOrderHeaderRepository orderHeader { get; }
        IOrderDetailRepository orderDetail { get; }
        void save();
        
    }
    
    
}
