using System.Collections.Generic;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface ICrudService<T>
    {
        IEnumerable<T> Get();

        T Get(int id);

        void Create(T item);

        void Update(T item);

        void Delete(int id);
    }
}