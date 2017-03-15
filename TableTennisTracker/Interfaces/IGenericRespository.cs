using System.Linq;

namespace TableTennisTracker.Interfaces
{
    public interface IGenericRespository
    {
        void Add<T>(T entityToCreate) where T : class;
        void Delete<T>(T entityToDelete) where T : class;
        void Dispose();
        IQueryable<T> Query<T>() where T : class;
        void SaveChanges();
    }
}