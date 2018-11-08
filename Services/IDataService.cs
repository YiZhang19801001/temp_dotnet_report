using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace demoBusinessReport.Services
{
    public interface IDataService<T>
    {
        Task<IEnumerable<T>> GetAll();
        void Create(T entity);
        Task<T> GetSingle(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Query(Expression<Func<T, bool>> predicate);
        void Update(T entity);
        void Delete(T entity);
    }
}
