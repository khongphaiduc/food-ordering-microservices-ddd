using System.Linq.Expressions;

namespace auth_services.AuthService.Domain.Interface
{
    public interface IRepository<T> where T : class
    {

        Task<IEnumerable<T>> GetAllAsync();

        Task AddAsync(T entity);

        void UpdateAsync(T entity);

        void DeleteAsync(T entity);

        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
    }
}
