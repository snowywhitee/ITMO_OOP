using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL
{
    public interface IRepository
    {
        void Add(IEntity entity);
        void Edit(IEntity entity);
        IEntity Get(int id);
        void Delete(IEntity entity);
        List<IEntity> GetAll();
    }
}
