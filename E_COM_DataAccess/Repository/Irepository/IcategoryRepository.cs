using E_COM_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_COM_DataAccess.Repository.Irepository
    //Access Generic repository in category model
{
    public interface IcategoryRepository:Irepository<Category>
    {
    }
}
