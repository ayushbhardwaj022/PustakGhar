using E_COM_DataAccess.Data;
using E_COM_DataAccess.Repository.Irepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_COM_DataAccess.Repository
{
    public class Repository<T> :Irepository <T> where T : class
    {
        private readonly ApplicationDbContext _context;//database connection
        internal DbSet<T> dbset;//table in db (T)means type=category or covertype
        public Repository(ApplicationDbContext context)//constructor

        {
            _context = context;
            dbset=_context.Set<T>();
        }
        public void Add(T entity)//Add/save  method ...will add new data to database tables
        {
           dbset.Add(entity);
        }

        public T FirstorDefult(Expression<Func<T, bool>> Filter = null,//single db tb//firstordefault= search without pk
            string includeproperties = null)//multiple tb db
        {
         IQueryable<T> query = dbset;//single table single record
            if (Filter != null)
                query = query.Where(Filter);
            if (includeproperties != null)//multiple tables data record search
            {
                foreach(var includeprop in includeproperties.Split(new[] {','},StringSplitOptions.RemoveEmptyEntries))
                {
                    query=query.Include(includeprop);
                }
            }
            return query.FirstOrDefault();
        }

        
        public T get(int id)//retrieve data by its ID/single data
        {
            return dbset.Find(id);
        }

        public IEnumerable<T> GetAll (Expression<Func<T, bool>> Filter = null, //retrieve multiple data eg:list from single tb
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, //filter/sort
            string includeproperties = null)
        {
            IQueryable<T> query = dbset;
            if(Filter!=null)
                query = query.Where(Filter);
           
            { 
                if(includeproperties!=null)//for data retreiving/filter  from multiple tb
                foreach (var includeprop in includeproperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) 
                {
                    query = query.Include(includeprop);

                }
                if(orderby!=null)
                    return orderby(query).ToList();//show in sortedway
                return query.ToList();///show all data

            }

        }

      

        public void Remove(T entity)//remove specific entity including data in it eg:(T = new Product { Id = 5, Name = "Mouse" });
        {
            dbset.Remove(entity);
        }

        public void Remove(int id)
        {
            dbset.Remove(get(id));//remove by primarykey column id
        }

        public void removerange(IEnumerable<T> entities)
        {
            dbset.RemoveRange(entities);//delete multiple entities at once
        }

        public void Update(T entity)
        {
            _context.ChangeTracker.Clear();  //avoid issues with duplicate tracking of the same entity
           //(e.g., if the entity is already tracked in the context, updating it again without detaching may cause errors).
            dbset.Update(entity);//update an existing record in the database
        }

        //void Irepository<T>.Add(T entity)
        //{
        //    throw new NotImplementedException();
        //}

        //T Irepository<T>.FirstorDefult(Expression<Func<T, bool>> Filter)
        //{
        //    throw new NotImplementedException();
        //}

        //T Irepository<T>.get(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerable<T> Irepository<T>.GetAll(Expression<Func<T, bool>> Filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderby, string includeproperties)
        //{
        //    throw new NotImplementedException();
        //}

        //void Irepository<T>.Remove(T entity)
        //{
        //    throw new NotImplementedException();
        //}

        //void Irepository<T>.Remove(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //void Irepository<T>.removerange(IEnumerable<T> entities)
        //{
        //    throw new NotImplementedException();
        //}

        //void Irepository<T>.Update(T entity)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
