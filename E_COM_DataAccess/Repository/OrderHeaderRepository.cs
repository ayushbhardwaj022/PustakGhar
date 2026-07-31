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
    public class OrderHeaderRepository:Repository<OrderHeader>,IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderHeaderRepository(ApplicationDbContext context):base(context) 
        {
            _context = context; 
            
        }
    }
}
