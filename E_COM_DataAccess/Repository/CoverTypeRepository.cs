using E_COM_DataAccess.Data;
using E_COM_DataAccess.Repository.Irepository;
using E_COM_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_COM_DataAccess.Repository
//Access Generic repository in category model
{
    public class CoverTypeRepository : Repository<CoverType>,ICoverTypeRepository
        
    {
        private readonly ApplicationDbContext _context;//database connection
        public CoverTypeRepository(ApplicationDbContext context) : base(context)//constructor
        {
            _context = context;
        }

        
     
    }
}
