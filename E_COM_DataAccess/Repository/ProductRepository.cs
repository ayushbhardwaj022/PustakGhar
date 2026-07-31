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
    public class ProductRepository : Repository<Product>, IProductRepository

    {
        private readonly ApplicationDbContext _context;//database connection
        public ProductRepository(ApplicationDbContext context) : base(context)//constructor
        {
            _context = context;
        }
    }
}
        
            
        
    

