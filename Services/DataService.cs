﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace demoBusinessReport.Services
{
    public class DataService<T> : IDataService<T> where T : class
    {
        private MyDbContext _context;
        private DbSet<T> _dbset;

        public DataService()
        {
            _context = new MyDbContext();
            _dbset = _context.Set<T>();
        }

        public async Task Create(T entity)
        {
            await _dbset.AddAsync(entity);
            await _context.SaveChangesAsync();//commit
        }

        public void Delete(T entity)
        {
            _dbset.Remove(entity);
            _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbset.ToListAsync();
        }

        public async Task<T> GetSingle(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public T GetSingleEntity(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> Query(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public void Update(T entity)
        {
            _dbset.Update(entity);
            _context.SaveChangesAsync();
        }
    }
}
