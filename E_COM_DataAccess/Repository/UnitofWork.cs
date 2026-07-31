using E_COM_DataAccess.Data;
using E_COM_DataAccess.Repository.Irepository;
using E_COM_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_COM_DataAccess.Repository
{
    public class UnitofWork : IUnitofWork
    {
        private readonly ApplicationDbContext _context;//database connection
        public UnitofWork(ApplicationDbContext context)//constructor
        {
            _context = context;
            category=new CategoryRepository(context);//Creates a category repo with the DB connection
            coverType =new CoverTypeRepository(context);//Creates a covertype repo with the DB connection
            product=new ProductRepository(context);//creates a product repository with db connection
            company=new CompanyRepository(context);
         ApplicationUser=new ApplicationUserRepository(context);
            shoppingCart=new ShoppingCartRepository(context);
            orderDetail=new OrderDetailRepository(context);
            orderHeader=new OrderHeaderRepository(context);
        }

        public IcategoryRepository category { private set; get; }   


        public ICoverTypeRepository coverType { private set; get; }
        public IProductRepository product { private set; get; }
        public ICompanyRepository company { private set; get; }
        public IApplicationUserRepository ApplicationUser{ private set; get; }
        public IShoppingCartRepository shoppingCart{ private set; get; }

       

        public IOrderDetailRepository orderDetail { private set; get; }
        public IOrderHeaderRepository orderHeader { private set; get; }

        

        public void save()
        {
           _context.SaveChanges();//apply all changes to db
        }
    }
}
