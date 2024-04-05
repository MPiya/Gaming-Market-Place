    using F2Play.DataAccess.Data;
using F2Play.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace F2Play.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        private readonly ILogger _logger;

        public Repository(ApplicationDbContext db, ILogger logger)
        {
            _db = db;
            _logger = logger;

            //_db.ShoppingCarts.Include(u => u.Product).Include(u=>u.CoverType);

            // _  _db.Set(T) is a generic class use to access any repository since its generic r
            dbSet = _db.Set<T>();
        }
        public void Add(T entity)
        {
            try
            {
                dbSet.Add(entity);
                _logger.Information("Added entity of type {EntityType}", typeof(T).Name);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error adding entity of type {EntityType}", typeof(T).Name);
                throw;
            }
        }
        //includeProp - "Category,CoverType"
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            try
            {
                IQueryable<T> query = dbSet;

                if (filter != null)
                {
                    query = query.Where(filter);
                }
                if (includeProperties != null)
                {
                    foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }

                _logger.Information("Retrieved entities of type {EntityType}", typeof(T).Name);

                return query.ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting entities of type {EntityType}", typeof(T).Name);
                throw;
            }
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true)
        {
            if (tracked)
            {
                IQueryable<T> query = dbSet;

                query = query.Where(filter);
                if (includeProperties != null)
                {
                    foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }
                return query.FirstOrDefault();
            }
            else
            {
                IQueryable<T> query = dbSet.AsNoTracking();

                query = query.Where(filter);
                if (includeProperties != null)
                {
                    foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }
                return query.FirstOrDefault();
            }

        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
