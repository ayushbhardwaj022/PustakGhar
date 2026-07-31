using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_COM_DataAccess.Repository.Irepository
{
    public interface Irepository<T> where T : class//T=type
    {
        void Add(T entity);//return type
        void Update(T entity);//return type
        void Remove(T entity);//return type
        void Remove(int id);//return type
        void removerange(IEnumerable<T> entities);//return type
        T get(int id);//find method//find work on primary key and single table
        IEnumerable<T> GetAll(Expression<Func<T,bool>>Filter=null,//display method
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null,//Sorting method
            string includeproperties=null//To access data from multiple tables// category and covertype table//
            
            );
        T FirstorDefult(Expression<Func<T, bool>> Filter = null, string includeproperties = null);//for find but firstordefault work on multipletables as category and covertype combine

    }
}
