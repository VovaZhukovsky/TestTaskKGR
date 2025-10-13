namespace TestTaskKGR.DAL;
public interface IRepository<T> : IDisposable where T : class
{
    IEnumerable<T> GetItemList();
    T? GetItem(int name);
    T Create(T item);
    void Update(T item);
    void Delete(int id);
    void Save();
}