using auth_services.AuthService.Domain.Interface;
using auth_services.AuthService.Infastructure.DbContextAuth;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace auth_services.AuthService.Infastructure.Reposistory
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly FoodAuthContext _db;

        public Repository(FoodAuthContext foodAuthContext)
        {
            _db = foodAuthContext;
        }

        public async Task AddAsync(T entity)
        {
            await _db.Set<T>().AddAsync(entity);
        }

        public void DeleteAsync(T entity)
        {

            _db.Set<T>().Remove(entity);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _db.Set<T>().Where(expression);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _db.Set<T>().ToListAsync();
        }


        public void UpdateAsync(T entity)
        {
            _db.Set<T>().Update(entity);
        }
    }
}
